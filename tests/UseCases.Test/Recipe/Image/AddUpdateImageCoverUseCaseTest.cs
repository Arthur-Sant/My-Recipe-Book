using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Storage;
using Microsoft.AspNetCore.Http;
using MyRecipeBook.Application.UseCases.Recipe.Image;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;
using UseCases.Test.Recipe.InlineData;

namespace UseCases.Test.Recipe.Image;

public class AddUpdateImageCoverUseCaseTest
{
    [Theory]
    [ClassData(typeof(ImageTypesInlineData))]
    public async Task Success(IFormFile file)
    {
        (var user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);

        var useCase = CreateUseCase(user, recipe);

        Func<Task> act = async () => await useCase.Execute(recipe.Id, file);

        await act.ShouldNotThrowAsync();
    }

    [Theory]
    [ClassData(typeof(ImageTypesInlineData))]
    public async Task Success_Recipe_Did_Not_Have_Image(IFormFile file)
    {
        (var user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);
        recipe.ImageIdentifier = null;

        var useCase = CreateUseCase(user, recipe);

        Func<Task> act = async () => await useCase.Execute(recipe.Id, file);

        await act.ShouldNotThrowAsync();

        recipe.ImageIdentifier.ShouldNotBeNullOrWhiteSpace();
    }

    [Theory]
    [ClassData(typeof(ImageTypesInlineData))]
    public async Task Error_Recipe_NotFound(IFormFile file)
    {
        (var user, _) = UserBuilder.Build();

        var useCase = CreateUseCase(user);

        var act = async () => await useCase.Execute(1, file);

        (await act.ShouldThrowAsync<NotFoundException>())
          .ShouldSatisfyAllConditions(
          e => e.Message.ShouldBe(ResourceMessagesException.RECIPE_NOT_FOUND)
          );
 
    }

    [Fact]
    public async Task Error_File_Is_Txt()
    {
        (var user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);

        var useCase = CreateUseCase(user, recipe);

        var file = FormFileBuilder.Txt();

        var act = async () => await useCase.Execute(recipe.Id, file);

        (await act.ShouldThrowAsync<ErrorOnValidationException>())
    .ShouldSatisfyAllConditions(
    e => e.GetErrorMessages().ShouldHaveSingleItem(),
    e => e.GetErrorMessages().ShouldContain(m => m.Equals(ResourceMessagesException.ONLY_IMAGES_ACCEPTED))
    );

    }

    private static AddUpdateImageCoverUseCase CreateUseCase(
        MyRecipeBook.Domain.Entities.User user,
        MyRecipeBook.Domain.Entities.Recipe? recipe = null)
    {
        var loggedUser = LoggedUserBuilder.Build(user);
        var repository = new RecipeUpdateOnlyRepositoryBuilder().GetById(user, recipe).Build();
        var blobStorage = new StorageServiceBuilder().Build();
        var unitOfWork = UnitOfWorkBuilder.Build();

        return new AddUpdateImageCoverUseCase(loggedUser, repository, unitOfWork, blobStorage);
    }
}

using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Storage;
using Microsoft.AspNetCore.Http;
using MyRecipeBook.Application.UseCases.Recipe.Register;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;
using UseCases.Test.Recipe.InlineData;

namespace UseCases.Test.Recipe.Register;

public class RegisterRecipeUseCaseTest
{
    [Fact]
    public async Task Success_Without_Image()
    {
        var (user, _) = UserBuilder.Build();

        var body = RequestRegisterRecipeFormDataBuilder.Build();

        var useCase = CreateUseCase(user);

        var result = await useCase.Execute(body);

        result.ShouldNotBeNull();
        result.Id.ShouldNotBeNullOrWhiteSpace();
        result.Title.ShouldBe(body.Title);
    }

    [Theory]
    [ClassData(typeof(ImageTypesInlineData))]
    public async Task Success_With_Image(IFormFile file)
    {
        (var user, _) = UserBuilder.Build();

        var request = RequestRegisterRecipeFormDataBuilder.Build(file);

        var useCase = CreateUseCase(user);

        var result = await useCase.Execute(request);

        result.ShouldNotBeNull();
        result.Id.ShouldNotBeNullOrWhiteSpace();
        result.Title.ShouldBe(request.Title);
    }

    [Fact]
    public async Task Error_Title_Empty()
    {
        var (user, _) = UserBuilder.Build();

        var body = RequestRegisterRecipeFormDataBuilder.Build();
        body.Title = string.Empty;

        var useCase = CreateUseCase(user);

        Func<Task> act = async () => { await useCase.Execute(body); };


        (await act.ShouldThrowAsync<ErrorOnValidationException>())
            .ShouldSatisfyAllConditions(
            e => e.GetErrorMessages().ShouldHaveSingleItem(),
            e => e.GetErrorMessages().ShouldContain(m => m.Equals(ResourceMessagesException.RECIPE_TITLE_EMPTY))
            );
    }

    [Fact]
    public async Task Error_Invalid_File()
    {
        (var user, _) = UserBuilder.Build();

        var textFile = FormFileBuilder.Txt();

        var request = RequestRegisterRecipeFormDataBuilder.Build(textFile);

        var useCase = CreateUseCase(user);

        var act = async () => { await useCase.Execute(request); };

        (await act.ShouldThrowAsync<ErrorOnValidationException>()).ShouldSatisfyAllConditions(
            e => e.GetErrorMessages().ShouldHaveSingleItem(),
            e => e.GetErrorMessages().ShouldContain(ResourceMessagesException.ONLY_IMAGES_ACCEPTED)
            );
    }

    private static RegisterRecipeUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user)
    {
        var mapper = MapperBuilder.Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        var repository = RecipeWriteOnlyRepositoryBuilder.Build();
        var storage = new StorageServiceBuilder().Build();

        return new RegisterRecipeUseCase(
            repository, 
            mapper,
            unitOfWork, 
            loggedUser,
            storage
            );
    }
}

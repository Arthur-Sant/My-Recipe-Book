using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Storage;
using MyRecipeBook.Application.UseCases.Recipe.GetById;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;

namespace UseCases.Test.Recipe.GetById;

public class GetRecipeByIdUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var ( user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);

        var useCase = CreateUseCase(user, recipe);

        var result = await useCase.Execute(recipe.Id);

        result.ShouldNotBeNull();
        result.Id.ShouldNotBeNullOrWhiteSpace();
        result.Title.ShouldBe(recipe.Title);
        result.ImageUrl.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Error_Recipe_NotFound()
    {
        var (user, _) = UserBuilder.Build();

        var useCase = CreateUseCase(user);

        Func<Task> act = async () => { await useCase.Execute(id: 1000); };

        (await act.ShouldThrowAsync<NotFoundException>())
            .ShouldSatisfyAllConditions(
            e => e.Message.ShouldBe(ResourceMessagesException.RECIPE_NOT_FOUND)
            );
    }

    private static GetRecipeByIdUseCase CreateUseCase(
        MyRecipeBook.Domain.Entities.User user,
        MyRecipeBook.Domain.Entities.Recipe? recipe = null)
    {
        var mapper = MapperBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        var repository = new RecipeReadOnlyRepositoryBuilder().GetById(user, recipe).Build();
        var storage = new StorageServiceBuilder().GetFileUrl(user, recipe?.ImageIdentifier).Build();

        return new GetRecipeByIdUseCase(mapper, loggedUser, repository, storage);
    }
}

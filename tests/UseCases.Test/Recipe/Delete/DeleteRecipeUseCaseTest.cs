using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Storage;
using MyRecipeBook.Application.UseCases.Recipe.Delete;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;

namespace UseCases.Test.Recipe.Delete;

public class DeleteRecipeUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var(user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);

        var useCase = CreateUseCase(user, recipe);

        var act = async () => { await useCase.Execute(recipe.Id); };

        await act.ShouldNotThrowAsync();
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

    private static DeleteRecipeUseCase CreateUseCase(
        MyRecipeBook.Domain.Entities.User user,
        MyRecipeBook.Domain.Entities.Recipe? recipe = null)
    {
        var loggedUser = LoggedUserBuilder.Build(user);
        var repositoryDelete = RecipeDeleteOnlyRepositoryBuilder.Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var storage = new StorageServiceBuilder().GetFileUrl(user, recipe?.ImageIdentifier).Build();
        var repositoryRead = new RecipeReadOnlyRepositoryBuilder().GetById(user, recipe).Build();

        return new DeleteRecipeUseCase(
            loggedUser, 
            repositoryRead, 
            repositoryDelete, 
            unitOfWork,
            storage
            );
    }
}

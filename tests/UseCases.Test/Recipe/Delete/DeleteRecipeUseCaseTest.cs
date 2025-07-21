using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using MyRecipeBook.Application.UseCases.Recipe.Delete;
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
        var repositoryRead = new RecipeReadOnlyRepositoryBuilder();
        var repositoryDelete = RecipeDeleteOnlyRepositoryBuilder.Build();
        var unitOfWork = UnitOfWorkBuilder.Build();

        if(recipe is not null)
            repositoryRead.RecipeExists(id: recipe.Id, userId: user.Id);

        return new DeleteRecipeUseCase(
            loggedUser, 
            repositoryRead.Build(), 
            repositoryDelete, 
            unitOfWork
            );
    }
}

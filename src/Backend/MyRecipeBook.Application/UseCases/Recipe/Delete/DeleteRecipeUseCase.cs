using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.Delete;

public class DeleteRecipeUseCase(
    ILoggedUser _loggedUser,
    IRecipeReadOnlyRepository _repositoryRead,
    IRecipeDeleteOnlyRepository _repositoryDelete,
    IUnityOfWork _unitOfWork
    ) : IDeleteRecipeUseCase
{
    public async Task Execute(long id)
    {
        var loggedUser = await _loggedUser.User();

        var recipeExist = await _repositoryRead.RecipeExists(id, loggedUser.Id);

        if(recipeExist.IsFalse())
            throw new NotFoundException(ResourceMessagesException.RECIPE_NOT_FOUND);

        _repositoryDelete.Delete(id);

        await _unitOfWork.Commit();
    }
}

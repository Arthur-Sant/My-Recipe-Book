using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Domain.Services.Storage;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.Delete;

public class DeleteRecipeUseCase(
    ILoggedUser _loggedUser,
    IRecipeReadOnlyRepository _repositoryRead,
    IRecipeDeleteOnlyRepository _repositoryDelete,
    IUnityOfWork _unitOfWork,
    IStorageService _storageService
    ) : IDeleteRecipeUseCase
{
    public async Task Execute(long id)
    {
        var loggedUser = await _loggedUser.User();

        var recipe = await _repositoryRead.GetById(loggedUser, id);

        if(recipe is null)
            throw new NotFoundException(ResourceMessagesException.RECIPE_NOT_FOUND);

        if(recipe.ImageIdentifier.NotEmpty())
            await _storageService.Delete(loggedUser, recipe.ImageIdentifier);

        _repositoryDelete.Delete(id);

        await _unitOfWork.Commit();
    }
}

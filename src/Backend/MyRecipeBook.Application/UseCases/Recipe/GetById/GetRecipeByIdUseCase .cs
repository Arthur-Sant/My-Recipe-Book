using AutoMapper;
using MyRecipeBook.Communication.Responses.Recipe;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Domain.Services.Storage;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.GetById;

public class GetRecipeByIdUseCase(
    IMapper _mapper,
    ILoggedUser _loggedUser,
    IRecipeReadOnlyRepository _repository,
    IStorageService _storageService
    ) : IGetRecipeByIdUseCase
{
    public async Task<ResponseRecipeJson> Execute(long id)
    {
        var loggedUser = await _loggedUser.User();

        var recipe = await _repository.GetById(loggedUser, id) ?? 
            throw new NotFoundException(ResourceMessagesException.RECIPE_NOT_FOUND);

        var response = _mapper.Map<ResponseRecipeJson>(recipe);

        if(recipe.ImageIdentifier.NotEmpty())
        {
            var url = await _storageService.GetFileUrl(loggedUser, recipe.ImageIdentifier);

            response.ImageUrl = url;
        }

        return response;
    }
}

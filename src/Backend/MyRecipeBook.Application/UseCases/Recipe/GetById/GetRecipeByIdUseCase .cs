using AutoMapper;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.GetById;

public class GetRecipeByIdUseCase(
    IMapper _mapper,
    ILoggedUser _loggedUser,
    IRecipeReadOnlyRepository _repository
    ) : IGetRecipeByIdUseCase
{
    public async Task<ResponseRecipeJson> Execute(long id)
    {
        var loggedUser = await _loggedUser.User();

        var recipe = await _repository.GetById(loggedUser, id) ?? 
            throw new NotFoundException(ResourceMessagesException.RECIPE_NOT_FOUND);

        return _mapper.Map<ResponseRecipeJson>(recipe);
    }
}

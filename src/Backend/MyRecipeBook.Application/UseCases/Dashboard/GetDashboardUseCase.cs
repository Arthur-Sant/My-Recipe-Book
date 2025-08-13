using AutoMapper;
using MyRecipeBook.Application.Extensions;
using MyRecipeBook.Communication.Responses.Recipe;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Domain.Services.Storage;

namespace MyRecipeBook.Application.UseCases.Dashboard;

public class GetDashboardUseCase(
    IRecipeReadOnlyRepository _repository,
    IMapper _mapper,
    ILoggedUser _loggedUser,
    IStorageService _storageService
    ) : IGetDashboardUseCase

{
    public async Task<ResponseRecipesJson> Execute()
    {
        var loggedUser = await _loggedUser.User();

        var recipes = await _repository.GetForDashboard(loggedUser);

        return new ResponseRecipesJson
        {
            Recipes = await recipes.MapToShortRecipeJson(loggedUser, _storageService, _mapper)
        };
    }
}

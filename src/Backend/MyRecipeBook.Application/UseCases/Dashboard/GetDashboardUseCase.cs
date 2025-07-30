using AutoMapper;
using MyRecipeBook.Communication.Responses.Recipe;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;

namespace MyRecipeBook.Application.UseCases.Dashboard;

public class GetDashboardUseCase(
    IRecipeReadOnlyRepository _repository,
    IMapper _mapper,
    ILoggedUser _loggedUser
    ) : IGetDashboardUseCase
{
    public async Task<ResponseRecipesJson> Execute()
    {
        var loggedUser = await _loggedUser.User();

        var recipes = await _repository.GetForDashboard(loggedUser);

        return new ResponseRecipesJson
        {
            Recipes =  _mapper.Map<IList<ResponseShortRecipeJson>>(recipes)
        };
    }
}

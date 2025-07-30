using MyRecipeBook.Communication.Responses.Recipe;

namespace MyRecipeBook.Application.UseCases.Dashboard;

public interface IGetDashboardUseCase
{
    public Task<ResponseRecipesJson> Execute();
}

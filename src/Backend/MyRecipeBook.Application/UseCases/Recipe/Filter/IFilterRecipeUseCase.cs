using MyRecipeBook.Communication.Requests.Recipe;
using MyRecipeBook.Communication.Responses.Recipe;

namespace MyRecipeBook.Application.UseCases.Recipe.Filter;

public interface IFilterRecipeUseCase
{
    public Task<ResponseRecipesJson> Execute(RequestFilterRecipeJson body); 
}

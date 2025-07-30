using MyRecipeBook.Communication.Responses.Recipe;

namespace MyRecipeBook.Application.UseCases.Recipe.GetById;

public interface IGetRecipeByIdUseCase
{
    public Task<ResponseRecipeJson> Execute(long id);
}

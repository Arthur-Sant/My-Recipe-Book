using MyRecipeBook.Communication.Requests.Token;
using MyRecipeBook.Communication.Responses.Token;

namespace MyRecipeBook.Application.UseCases.Token.Refresh;

public interface IRefreshTokenUseCase
{
    public Task<ResponseTokensJson> Execute(RequestNewTokenJson request);
}

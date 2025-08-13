using MyRecipeBook.Communication.Requests.Login;
using MyRecipeBook.Communication.Responses.User;

namespace MyRecipeBook.Application.UseCases.Login.DoLogin;
public interface IDoLoginUseCase
{
    public Task<ResponseRegisterUserJson> Execute(RequestLoginJson body);
}

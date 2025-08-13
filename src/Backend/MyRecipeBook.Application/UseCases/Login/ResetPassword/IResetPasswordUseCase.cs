using MyRecipeBook.Communication.Requests.Login;

namespace MyRecipeBook.Application.UseCases.Login.ResetPassword;

public interface IResetPasswordUseCase
{
    public Task Execute(RequestResetPasswordJson body);
}

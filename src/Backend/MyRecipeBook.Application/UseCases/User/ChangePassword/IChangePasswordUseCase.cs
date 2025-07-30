using MyRecipeBook.Communication.Requests.User;

namespace MyRecipeBook.Application.UseCases.User.ChangePassword;
public interface IChangePasswordUseCase
{
    public Task Execute(RequestChangePasswordJson body);
}

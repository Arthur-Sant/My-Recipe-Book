using MyRecipeBook.Communication.Requests.User;

namespace MyRecipeBook.Application.UseCases.User.Update;
public interface IUpdateUserUsecase
{
    public Task Execute(RequestUpdateUserJson body);
}

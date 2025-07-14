using MyRecipeBook.Communication.Requests;

namespace MyRecipeBook.Application.UseCases.User.Update;
public interface IUpdateUserUsecase
{
    public Task Execute(RequestUpdateUserJson body);
}

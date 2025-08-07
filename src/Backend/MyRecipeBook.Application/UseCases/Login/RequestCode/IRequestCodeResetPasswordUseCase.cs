namespace MyRecipeBook.Application.UseCases.Login.RequestCode;


public interface IRequestCodeResetPasswordUseCase
{
    public Task Execute(string email);
}

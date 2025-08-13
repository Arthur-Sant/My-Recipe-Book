namespace MyRecipeBook.Application.UseCases.Login.External;

public interface IExternalLoginUseCase
{
    public Task<string> Execute(string name, string email);
}

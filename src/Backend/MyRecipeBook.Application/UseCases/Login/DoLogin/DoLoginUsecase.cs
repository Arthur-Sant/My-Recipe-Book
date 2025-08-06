using MyRecipeBook.Communication.Requests.Login;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Communication.Responses.User;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.Cryptography;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Login.DoLogin;
public class DoLoginUsecase(
    IUserReadOnlyRepository _repository, 
    IPasswordEncripter _passwordEncripter, 
    IAccessTokenGenerator _acessTokenGenerator
    ) : IDoLoginUseCase
{

    public async Task<ResponseRegisterUserJson> Execute(RequestLoginJson body)
    {
        var encriptedPassword = _passwordEncripter.Encrypt(body.Password);

        var user = await _repository.GetByEmailAndPassword(body.Email, encriptedPassword) ?? 
            throw new InvalidLoginException();

        return new ResponseRegisterUserJson
        {
            Name = user.Name,
            Tokens = new ResponseTokensJson
            {
                AccessToken = _acessTokenGenerator.Generate(user.UserIdentifier)
            }
        };
    }
}

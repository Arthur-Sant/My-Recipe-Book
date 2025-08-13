using MyRecipeBook.Communication.Requests.Login;
using MyRecipeBook.Communication.Responses.Token;
using MyRecipeBook.Communication.Responses.User;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Token;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.Cryptography;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Login.DoLogin;
public class DoLoginUsecase(
    IUserReadOnlyRepository _repository,
    ITokenWriteOnlyRepository _tokenWriteRepository,
    IUnityOfWork _unityOfWork,
    IPasswordEncripter _passwordEncripter, 
    IAccessTokenGenerator _acessTokenGenerator,
        IRefreshTokenGenerator _refreshTokenGenerator

    ) : IDoLoginUseCase
{

    public async Task<ResponseRegisterUserJson> Execute(RequestLoginJson body)
    {
        var user = await _repository.GetByEmail(body.Email); 

        if(user is null || _passwordEncripter.IsValid(body.Password, user.Password).IsFalse())
            throw new InvalidLoginException();

        var refreshToken = await CreateAndSaveRefreshToken(user);

        return new ResponseRegisterUserJson
        {
            Name = user.Name,
            Tokens = new ResponseTokensJson
            {
                AccessToken = _acessTokenGenerator.Generate(user.UserIdentifier),
                RefreshToken = refreshToken
            }
        };

    }

    private async Task<string> CreateAndSaveRefreshToken(Domain.Entities.User user)
    {
        var refreshToken = _refreshTokenGenerator.Generate();

        await _tokenWriteRepository.Register(new Domain.Entities.Token
        {
            Value = refreshToken,
            UserId = user.Id
        });

        await _unityOfWork.Commit();

        return refreshToken;
    }
}

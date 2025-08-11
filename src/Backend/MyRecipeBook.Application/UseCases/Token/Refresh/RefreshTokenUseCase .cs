using MyRecipeBook.Communication.Requests.Token;
using MyRecipeBook.Communication.Responses.Token;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Token;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Domain.ValueObjects;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Token.Refresh;

public class RefreshTokenUseCase(
    IUnityOfWork _unitOfWork,
    ITokenReadOnlyRepository _tokenReadRepository,
    ITokenWriteOnlyRepository _tokenWriteRepository,
    ITokenDeleteOnlyRepository _tokenDeleteRepository,
    IRefreshTokenGenerator _refreshTokenGenerator,
    IAccessTokenGenerator _accessTokenGenerator
    ) : IRefreshTokenUseCase
{
    public async Task<ResponseTokensJson> Execute(RequestNewTokenJson request)
    {
        var refreshToken = await _tokenReadRepository.Get(request.RefreshToken);

        if(refreshToken is null)
            throw new RefreshTokenNotFoundException();

        var refreshTokenValidUntil = refreshToken.CreatedAt.AddDays(MyRecipeBookRuleConstants.REFRESH_TOKEN_EXPIRATION_DAYS);
        if(DateTime.Compare(refreshTokenValidUntil, DateTime.UtcNow) < 0)
            throw new RefreshTokenExpiredException();

        var newRefreshToken = new Domain.Entities.Token
        {
            Value = _refreshTokenGenerator.Generate(),
            UserId = refreshToken.UserId
        };

        await _tokenDeleteRepository.DeleteAllByUserId(refreshToken.UserId);

        await _tokenWriteRepository.Register(newRefreshToken);

        await _unitOfWork.Commit();

        return new ResponseTokensJson
        {
            AccessToken = _accessTokenGenerator.Generate(refreshToken.User.UserIdentifier),
            RefreshToken = newRefreshToken.Value
        };
    }
}

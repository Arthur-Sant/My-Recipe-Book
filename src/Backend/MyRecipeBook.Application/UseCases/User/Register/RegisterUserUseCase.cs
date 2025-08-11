using AutoMapper;
using MyRecipeBook.Communication.Requests.User;
using MyRecipeBook.Communication.Responses.Token;
using MyRecipeBook.Communication.Responses.User;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Token;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.Cryptography;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.User.Register;

public class RegisterUserUseCase(
    IUserWriteOnlyRepository _writeonlyRepository,
    IUserReadOnlyRepository _readonlyRepository,
    ITokenWriteOnlyRepository _tokenWriteRepository,
    IMapper _mapper,
    IPasswordEncripter _passwordEncripter,
    IUnityOfWork _unityOfWork,
    IAccessTokenGenerator _acessTokenGenerator,
    IRefreshTokenGenerator _refreshTokenGenerator
        ) : IRegisterUseCase
{
    public async Task<ResponseRegisterUserJson> Execute(RequestRegisterUserJson body)
    {
        await Validate(body);

        var user = _mapper.Map<Domain.Entities.User>(body);
        user.Password = _passwordEncripter.Encrypt(body.Password);
        user.UserIdentifier = Guid.NewGuid();

        await _writeonlyRepository.Add(user);

        await _unityOfWork.Commit();

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

    private async Task Validate(RequestRegisterUserJson body)
    {
        var result = new RegisterUserValidator().Validate(body);

        var emailExist = await _readonlyRepository.ExistActiveUserWithEmail(body.Email);

        if(emailExist)
        {
            result.Errors.Add(new FluentValidation.Results.ValidationFailure(string.Empty, ResourceMessagesException.EMAIL_ALREADY_REGISTERED));
        }

        if(result.IsValid.IsFalse())
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}

using FluentValidation.Results;
using MyRecipeBook.Communication.Requests.Login;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.CodeToPerformAction;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.Cryptography;
using MyRecipeBook.Domain.ValueObjects;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Login.ResetPassword;

public class ResetPasswordUseCase (
        IPasswordEncripter _passwordEncripter,
        IUserUpdateOnlyRepository _userUpdateRepository,
        ICodeToPerformActionReadOnlyRepository _codeReadRepository,
        IUnityOfWork _unityOfWork
    )
    : IResetPasswordUseCase
{
    public async Task Execute(RequestResetPasswordJson body)
    {
        var code = await _codeReadRepository.GetByCode(body.Code);

        if(code is null)
            throw new ErrorOnValidationException([ResourceMessagesException.CODE_RESET_PASSWORD_REQUIRED]);

        var user = await _userUpdateRepository.GetById(code.UserId);

        Validate(user, code, body);

        user.Password = _passwordEncripter.Encrypt(body.NewPassword);

        _userUpdateRepository.Update(user);

        await _unityOfWork.Commit();
    }

    private static void Validate(
        Domain.Entities.User? user,
        Domain.Entities.CodeToPerformAction code,
        RequestResetPasswordJson body
        )
    {
        if(user is null || user.Email.Equals(body.Email).IsFalse())
            throw new ErrorOnValidationException([ResourceMessagesException.CODE_INVALID]);

        var validation = new ResetPasswordValidation().Validate(body);

        if(code.CreatedAt >= DateTime.UtcNow.AddMinutes(- MyRecipeBookRuleConstants.PASSWORD_RESET_CODE_VALIDITY_MINUTES))
            validation.Errors.Add(new ValidationFailure("ExpiredCode", ResourceMessagesException.EXPIRED_CODE));

        if(validation.IsValid.IsFalse())
            throw new ErrorOnValidationException(validation.Errors.Select(c => c.ErrorMessage).ToList());
    }
}

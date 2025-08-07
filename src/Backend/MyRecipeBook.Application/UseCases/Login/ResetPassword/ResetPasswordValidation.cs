using FluentValidation;
using MyRecipeBook.Application.Shared.Validators;
using MyRecipeBook.Communication.Requests.Login;
using MyRecipeBook.Communication.Requests.User;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.ValueObjects;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Login.ResetPassword;

public class ResetPasswordValidation : AbstractValidator<RequestResetPasswordJson>
{
    public ResetPasswordValidation()
    {
        RuleFor(user => user.NewPassword).SetValidator(new PasswordValidator<RequestResetPasswordJson>());
        RuleFor(user => user.Code)
            .NotEmpty()
            .Length(MyRecipeBookRuleConstants.NUMBER_CHARACTERS_FOR_ACTION_CODE)
            .WithMessage(ResourceMessagesException.CODE_INVALID);
    }
}

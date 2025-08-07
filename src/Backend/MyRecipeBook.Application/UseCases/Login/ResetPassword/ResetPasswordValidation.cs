using FluentValidation;
using MyRecipeBook.Application.Shared.Validators;
using MyRecipeBook.Communication.Requests.Login;
using MyRecipeBook.Communication.Requests.User;
using MyRecipeBook.Domain.Entities;

namespace MyRecipeBook.Application.UseCases.Login.ResetPassword;

public class ResetPasswordValidation : AbstractValidator<RequestResetPasswordJson>
{
    public ResetPasswordValidation()
    {
        RuleFor(user => user.NewPassword).SetValidator(new PasswordValidator<RequestResetPasswordJson>());
    }
}

using FluentValidation;
using MyRecipeBook.Application.Shared.Validators;
using MyRecipeBook.Communication.Requests.User;

namespace MyRecipeBook.Application.UseCases.User.ChangePassword;
public class ChangePasswordValidator : AbstractValidator<RequestChangePasswordJson>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.NewPassword).SetValidator(new PasswordValidator<RequestChangePasswordJson>()); 
    }
}

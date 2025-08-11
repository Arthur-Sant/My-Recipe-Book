using MyRecipeBook.Communication.Requests.User;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.Cryptography;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.User.ChangePassword;
public class ChangePasswordUseCase(
    ILoggedUser _loggedUser,
    IPasswordEncripter _passwordEncripter,
    IUserUpdateOnlyRepository _repository,
    IUnityOfWork _unityOfWork
    ) : IChangePasswordUseCase
{
    public async Task Execute(RequestChangePasswordJson body)
    {
        var loggedUser = await _loggedUser.User();

        Validate(body, loggedUser);

        var user = await _repository.GetById(loggedUser.Id);

        user.Password = _passwordEncripter.Encrypt(body.NewPassword);

        _repository.Update(user);

        await _unityOfWork.Commit();
    }

    private void Validate(RequestChangePasswordJson body, Domain.Entities.User loggedUser)
    {
        var result = new ChangePasswordValidator().Validate(body);

        if(_passwordEncripter.IsValid(body.Password, loggedUser.Password).IsFalse())
            result.Errors.Add(new FluentValidation.Results.ValidationFailure(string.Empty, ResourceMessagesException.CURRENT_PASSWORD_INCORRECT));

        if(result.IsValid.IsFalse())
            throw new ErrorOnValidationException(result.Errors.Select(e => e.ErrorMessage).ToList());
    }
}

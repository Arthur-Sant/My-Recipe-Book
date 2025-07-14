using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.User.Update;
public class UpdateUserUsecase(
    ILoggedUser _loggedUser,
    IUserUpdateOnlyRepository _repository,
    IUnityOfWork _unityOfWork,
    IUserReadOnlyRepository _userReadRepository
    ) : IUpdateUserUsecase
{
    public async Task Execute(RequestUpdateUserJson body)
    {
        var loggedUser = await _loggedUser.User();

        await Validate(body, loggedUser.Email);

        var user = await _repository.GetById(loggedUser.Id);

        user.Email = body.Email;
        user.Name = body.Name;

        _repository.Update(user);

        await _unityOfWork.Commit();
    }

    private async Task Validate(RequestUpdateUserJson body, string currentEmail)
    {
        var validator = new UpdateUserValidator();

        var result = validator.Validate(body);

        if(currentEmail.Equals(body.Email).IsFalse())
        {
            var userExist = await _userReadRepository.ExistActiveUserWithEmail(body.Email);
            if(userExist)
                result.Errors.Add(new FluentValidation.Results.ValidationFailure("email", ResourceMessagesException.EMAIL_ALREADY_REGISTERED));

            if(result.IsValid.IsFalse())
            {
                var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();

                throw new ErrorOnValidationException(errorMessages);
            }
        }
    }
}

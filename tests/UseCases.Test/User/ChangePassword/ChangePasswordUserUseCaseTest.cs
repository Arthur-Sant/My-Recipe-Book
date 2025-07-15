using CommonTestUtilities.Cryptography;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.User.ChangePassword;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;

namespace UseCases.Test.User.ChangePassword;
public class ChangePasswordUserUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var (user, password) = UserBuilder.Build();

        var body = RequestChangePasswordJsonBuilder.Build();
       body.Password = password;

        var useCase = CreateUseCase(user);

        Func<Task> act = async () => await useCase.Execute(body);

        await act.ShouldNotThrowAsync();

        var passwordEncripter = PasswordEncrypterBuilder.Build();

        user.Password.ShouldBe(passwordEncripter.Encrypt(body.NewPassword));
    }

    [Fact]
    public async Task Error_NewPassword_Empty()
    {
        var (user, password) = UserBuilder.Build();

        var body = new RequestChangePasswordJson
        {
            Password = password,
            NewPassword = string.Empty
        };
        
        var useCase = CreateUseCase(user);

        Func<Task> act = async () => await useCase.Execute(body);

        (await act.ShouldThrowAsync<ErrorOnValidationException>())
            .ShouldSatisfyAllConditions(
            e => e.ErrorMessages.ShouldHaveSingleItem(),
            e => e.ErrorMessages.ShouldContain(m => m.Equals(ResourceMessagesException.PASSWORD_EMPTY))
            );

        var passwordEncripter = PasswordEncrypterBuilder.Build();

        user.Password.ShouldNotBe(passwordEncripter.Encrypt(body.NewPassword));
    }

    [Fact]
    public async Task Error_CurrentPassword_Different()
    {
        var (user, password) = UserBuilder.Build();

        var body = RequestChangePasswordJsonBuilder.Build();

        var useCase = CreateUseCase(user);

        Func<Task> act = async () => await useCase.Execute(body);

        (await act.ShouldThrowAsync<ErrorOnValidationException>())
            .ShouldSatisfyAllConditions(
            e => e.ErrorMessages.ShouldHaveSingleItem(),
            e => e.ErrorMessages.ShouldContain(m => m.Equals(ResourceMessagesException.CURRENT_PASSWORD_INCORRECT))
            );

        var passwordEncripter = PasswordEncrypterBuilder.Build();

        user.Password.ShouldNotBe(passwordEncripter.Encrypt(body.NewPassword));
    }

    private static ChangePasswordUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user)
    {
        var loggedUser = LoggedUserBuilder.Build(user);
        var unitOfWork = UnitOfWorkBuilder.Build();
        var userUpdateRepository = new UserUpdateOnlyRepositoryBuilder().GetById(user).Build();
        var passwordEncripter = PasswordEncrypterBuilder.Build();

        return new ChangePasswordUseCase(
            loggedUser,
            passwordEncripter,
            userUpdateRepository,
            unitOfWork
            );

    }
}

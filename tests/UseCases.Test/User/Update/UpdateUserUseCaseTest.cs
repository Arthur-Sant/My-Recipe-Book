using CommonTestUtilities.Cryptography;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using MyRecipeBook.Application.UseCases.User.Update;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;

namespace UseCases.Test.User.Update;
public class UpdateUserUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();

        var body = RequestUpdateUserJsonBuilder.Build();

        var usecase = CreateUseCase(user);

        Func<Task> act = async () => await usecase.Execute(body);

        await act.ShouldNotThrowAsync();

        user.Email.ShouldBe(body.Email);
        user.Name.ShouldBe(body.Name);
    }

    [Fact]
    public async Task Error_Name_Empty()
    {
        var (user, _) = UserBuilder.Build();

        var body = RequestUpdateUserJsonBuilder.Build();
        body.Name = string.Empty;

        var usecase = CreateUseCase(user);

        Func<Task> act = async () => await usecase.Execute(body);

        (await act.ShouldThrowAsync<ErrorOnValidationException>())
            .ShouldSatisfyAllConditions(
            e => e.GetErrorMessages().ShouldHaveSingleItem(),
            e => e.GetErrorMessages().ShouldContain(m => m.Equals(ResourceMessagesException.NAME_EMPTY))
            );

        user.Email.ShouldNotBe(body.Email);
        user.Name.ShouldNotBe(body.Name);
    }

    [Fact]
    public async Task Error_Email_Already_Registered()
    {
        var (user, _) = UserBuilder.Build();

        var body = RequestUpdateUserJsonBuilder.Build();

        var usecase = CreateUseCase(user, body.Email);

        Func<Task> act = async () => await usecase.Execute(body);

        (await act.ShouldThrowAsync<ErrorOnValidationException>())
            .ShouldSatisfyAllConditions(
            e => e.GetErrorMessages().ShouldHaveSingleItem(),
            e => e.GetErrorMessages().ShouldContain(m => m.Equals(ResourceMessagesException.EMAIL_ALREADY_REGISTERED))
            );

        user.Email.ShouldNotBe(body.Email);
        user.Name.ShouldNotBe(body.Name);
    }

    private static UpdateUserUsecase CreateUseCase(MyRecipeBook.Domain.Entities.User user, string? email = null)
    {
        var loggedUser = LoggedUserBuilder.Build(user);
        var unitOfWork = UnitOfWorkBuilder.Build();
        var userUpdateRepository = new UserUpdateOnlyRepositoryBuilder().GetById(user).Build();
        var readRepositoryBuilder = new UserReadOnlyRepositoryBuilder();

        if(string.IsNullOrEmpty(email) == false)
            readRepositoryBuilder.ExistActiveUserWithEmail(email);

        return new UpdateUserUsecase(
            loggedUser,
            userUpdateRepository,
            unitOfWork,
            readRepositoryBuilder.Build()
            );
    }
}

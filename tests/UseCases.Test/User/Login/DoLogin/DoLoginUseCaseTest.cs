using CommonTestUtilities.Cryptography;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.User.Login.DoLogin;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;

namespace UseCases.Test.User.Login.DoLogin;
public class DoLoginUseCaseTest
{

    [Fact]
    public async Task Success()
    {
        var (user, password) = UserBuilder.Build();

        var body = new RequestLoginJson
        {
            Password = password,
            Email = user.Email
        };

        var useCase = CreateUseCase(user);

        var result = await useCase.Execute(body);

        result.ShouldNotBeNull();
        result.Name.ShouldSatisfyAllConditions(
            n => n.ShouldNotBeNullOrWhiteSpace(),
            n => n.ShouldBe(user.Name)
        ); 
    }

    [Fact]
    public async Task Error_Invalid_User()
    {
        var body = RequestLoginJsonBuilder.Build();

        var useCase = CreateUseCase();

        Func<Task> act = async () => { await useCase.Execute(body); };

        (await act.ShouldThrowAsync<InvalidLoginException>())
            .ShouldSatisfyAllConditions(
                e => e.Message.ShouldBe(ResourceMessagesException.EMAIL_OR_PASSWORD_INVALID)
            );

    }

    private static DoLoginUsecase CreateUseCase(MyRecipeBook.Domain.Entities.User? user = null)
    {
        var passwordEncripter = PasswordEncrypterBuilder.Build();
        var userReadRepositoryBuilder = new UserReadOnlyRepositoryBuilder();

        if(user is not null)
            userReadRepositoryBuilder.GetByEmailAndPassword(user);

        return new DoLoginUsecase(userReadRepositoryBuilder.Build(), passwordEncripter);
    }
}

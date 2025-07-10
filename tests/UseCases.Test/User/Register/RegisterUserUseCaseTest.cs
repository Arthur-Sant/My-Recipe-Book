using CommonTestUtilities.Cryptography;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using MyRecipeBook.Application.UseCases.User.Register;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;

namespace UseCases.Test.User.Register;
public class RegisterUserUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var body = RequestRegisterUserJsonBuilder.Build();

        var useCase = CreateUseCase();

        var result = await useCase.Execute(body);

        result.ShouldNotBeNull();
        result.Tokens.ShouldNotBeNull();
        result.Name.ShouldBe(body.Name);
        result.Tokens.AccessToken.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Error_Email_Already_Registered()
    {
        var body = RequestRegisterUserJsonBuilder.Build();

        var useCase = CreateUseCase(body.Email);

        Func<Task> act = async () => await useCase.Execute(body);

        (await act.ShouldThrowAsync<ErrorOnValidationException>())
            .ShouldSatisfyAllConditions(
            e => e.ErrorMessages.ShouldHaveSingleItem(),
            e => e.ErrorMessages.ShouldContain(m => m.Equals(ResourceMessagesException.EMAIL_ALREADY_REGISTERED))
            ); 

    }

    [Fact]
    public async Task Error_Name_Empty()
    {
        var body = RequestRegisterUserJsonBuilder.Build();
        body.Name = string.Empty;

        var useCase = CreateUseCase();

        Func<Task> act = async () => await useCase.Execute(body);

        (await act.ShouldThrowAsync<ErrorOnValidationException>())
            .ShouldSatisfyAllConditions(
            e => e.ErrorMessages.ShouldHaveSingleItem(),
            e => e.ErrorMessages.ShouldContain(m => m.Equals(ResourceMessagesException.NAME_EMPTY))
            );

    }

    private static RegisterUserUseCase CreateUseCase(string? email = null)
    {
        var unitOfWork = UnitOfWorkBuilder.Build();
        var mapper = MapperBuilder.Build();
        var passwordEncripter = PasswordEncrypterBuilder.Build();
        var writeRepository = UserWriteOnlyRepositoryBuilder.Build();
        var readRepositoryBuilder = new UserReadOnlyRepositoryBuilder();
        var accessTokenGenerator = JwtTokenGeneratorBuilder.Build();

        if(string.IsNullOrEmpty(email) == false)
            readRepositoryBuilder.ExistActiveUserWithEmail(email);

        return  new RegisterUserUseCase(
            writeRepository,
            readRepositoryBuilder.Build(),
            mapper,
            passwordEncripter,
            unitOfWork,
            accessTokenGenerator
        );
    }
}

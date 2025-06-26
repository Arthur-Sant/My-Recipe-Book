using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.User.Register;
using MyRecipeBook.Exceptions;
using Shouldly;

namespace Validators.Test.User.Register;

public class RegisterUserValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new RegisterUserValidator();

        var body = RequestRegisterUserJsonBuilder.Build();

        var result = validator.Validate(body);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Error_Name_Empty() {
        var validator = new RegisterUserValidator();

        var body = RequestRegisterUserJsonBuilder.Build();
        body.Name = string.Empty;

        var result = validator.Validate(body);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
            e => e.ShouldHaveSingleItem(),
            e => e.ShouldContain(m => m.ErrorMessage.Equals(ResourceMessagesException.NAME_EMPTY))
            );
    }

    [Fact]
    public void Error_Email_Empty()
    {
        var validator = new RegisterUserValidator();

        var body = RequestRegisterUserJsonBuilder.Build();
        body.Email = string.Empty;

        var result = validator.Validate(body);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
            e => e.ShouldHaveSingleItem(),
            e => e.ShouldContain(m => m.ErrorMessage.Equals(ResourceMessagesException.EMAIL_EMPTY))
            );
    }

    [Fact]
    public void Error_Email_Invalid()
    {
        var validator = new RegisterUserValidator();

        var body = RequestRegisterUserJsonBuilder.Build();
        body.Email = "email.com";

        var result = validator.Validate(body);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
            e => e.ShouldHaveSingleItem(),
            e => e.ShouldContain(m => m.ErrorMessage.Equals(ResourceMessagesException.EMAIL_INVALID))
            );
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void Error_Password_Invalid(int passwordsLenght)
    {
        var validator = new RegisterUserValidator();

        var body = RequestRegisterUserJsonBuilder.Build(passwordsLenght);
        
        var result = validator.Validate(body);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
            e => e.ShouldHaveSingleItem(),
            e => e.ShouldContain(m => m.ErrorMessage.Equals(ResourceMessagesException.PASSWORD_LENGTH))
            );
    }
}

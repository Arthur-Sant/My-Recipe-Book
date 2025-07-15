using CommonTestUtilities.Requests;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using MyRecipeBook.Application.UseCases.User.ChangePassword;
using MyRecipeBook.Exceptions;
using Shouldly;

namespace Validators.Test.User.ChangePassword;
public class ChangePasswordValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new ChangePasswordValidator();

        var body = RequestChangePasswordJsonBuilder.Build();

        var result = validator.Validate(body);

        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void Error_Password_Invalid(int passwordLenght)
    {
        var validator = new ChangePasswordValidator();

        var body = RequestChangePasswordJsonBuilder.Build(passwordLenght);

        var result = validator.Validate(body);

        result.IsValid.ShouldBeFalse();

        result.Errors.ShouldSatisfyAllConditions(
            e => e.ShouldHaveSingleItem(),
            e => e.ShouldContain(m => m.ErrorMessage.Equals(ResourceMessagesException.PASSWORD_LENGTH))
            );
    }

    [Fact]
    public void Error_Password_Empty()
    {
        var validator = new ChangePasswordValidator();

        var body = RequestChangePasswordJsonBuilder.Build();
        body.NewPassword = string.Empty;

        var result = validator.Validate(body);

        result.IsValid.ShouldBeFalse();

        result.Errors.ShouldSatisfyAllConditions(
            e => e.ShouldHaveSingleItem(),
            e => e.ShouldContain(m => m.ErrorMessage.Equals(ResourceMessagesException.PASSWORD_EMPTY))
            );
    }
}

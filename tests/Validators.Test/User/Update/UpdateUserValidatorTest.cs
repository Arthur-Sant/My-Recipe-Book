using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.User.Register;
using MyRecipeBook.Application.UseCases.User.Update;
using MyRecipeBook.Exceptions;
using Shouldly;

namespace Validators.Test.User.Update;
public class UpdateUserValidatorTest
{

    [Fact]
    public void Success()
    {
        var validator = new UpdateUserValidator();

        var body = RequestUpdateUserJsonBuilder.Build();

        var result = validator.Validate(body);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Error_Name_Empty()
    {
        var validator = new UpdateUserValidator();

        var body = RequestUpdateUserJsonBuilder.Build();
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
        var validator = new UpdateUserValidator();

        var body = RequestUpdateUserJsonBuilder.Build();
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
        var validator = new UpdateUserValidator();

        var body = RequestUpdateUserJsonBuilder.Build();
        body.Email = "email.com";

        var result = validator.Validate(body);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
            e => e.ShouldHaveSingleItem(),
            e => e.ShouldContain(m => m.ErrorMessage.Equals(ResourceMessagesException.EMAIL_INVALID))
            );
    }
}

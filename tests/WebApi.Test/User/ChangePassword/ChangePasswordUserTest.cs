using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using MyRecipeBook.Communication.Requests.User;
using MyRecipeBook.Communication.Requests.Login;
using MyRecipeBook.Exceptions;
using Shouldly;
using System.Globalization;
using System.Net;
using WebApi.Test.InlineData;

namespace WebApi.Test.User.ChangePassword;
public class ChangePasswordUserTest : MyRecipeBookClassFixture
{
    private readonly string _route = "user/change-password",
        _password,
        _email;

    private readonly Guid _userIdentifier;

    public ChangePasswordUserTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _password = factory.GetPassword();
        _email = factory.GetEmail();
        _userIdentifier = factory.GetUserIdentifier();
    }

    [Fact]
    public async Task Success()
    {
        var body = RequestChangePasswordJsonBuilder.Build();
        body.Password = _password;

        var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

        var response = await DoPatch(_route, body, token);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var loginBody = new RequestLoginJson
        {
            Email = _email,
            Password = _password
        };

        response = await DoPost(route: "login", loginBody);
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        loginBody.Password = body.NewPassword;

        response = await DoPost(route: "login", loginBody);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Theory]
    [ClassData(typeof(CultueInlineDataTest))]
    public async Task Error_NewPassword_Empty(string culture)
    {
        var body = new RequestChangePasswordJson
        {
            Password = _password,
            NewPassword = string.Empty
        };

        var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

        var response = await DoPatch(_route, body, token, culture);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var errors = await GetErrosFromResponse(response);

        var expectedMessage = ResourceMessagesException.ResourceManager.GetString("PASSWORD_EMPTY", new CultureInfo(culture))!;

        errors.ShouldSatisfyAllConditions(
            v => v.ShouldHaveSingleItem(),
            v => v.ShouldContain(e => e.GetString()!.Equals(expectedMessage))
            );

    }
}

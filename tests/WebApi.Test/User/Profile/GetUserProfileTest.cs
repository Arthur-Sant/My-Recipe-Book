using CommonTestUtilities.Tokens;
using Shouldly;
using System.Net;
using System.Text.Json;

namespace WebApi.Test.User.Profile;
public class GetUserProfileTest : MyRecipeBookClassFixture
{
    private readonly string _route = "user", _name, _email;

    private readonly Guid _userIdentifier;

    public GetUserProfileTest(CustomWebApplicationFactory _factory) : base(_factory)
    {
        _name = _factory.GetName();
        _email = _factory.GetEmail();
        _userIdentifier = _factory.GetUserIdentifier();
    }


    [Fact]
    public async Task Sucess()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

        var response = await DoGet(_route, token);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        responseData.RootElement.GetProperty("name").GetString().ShouldSatisfyAllConditions(
            n => n.ShouldBe(_name),
            n => n.ShouldNotBeNullOrWhiteSpace()
            );

        responseData.RootElement.GetProperty("email").GetString().ShouldSatisfyAllConditions(
            n => n.ShouldBe(_email),
            n => n.ShouldNotBeNullOrWhiteSpace()
            );
    }
}

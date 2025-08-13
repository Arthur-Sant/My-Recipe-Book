using CommonTestUtilities.Tokens;
using Shouldly;
using System.Net;
using System.Text.Json;

namespace WebApi.Test.Dashboard;

public class GetDashboardTest : MyRecipeBookClassFixture
{
    private readonly string _route = "dashboard",
        _recipeId;

    private readonly Guid _userIdentifier;

    public GetDashboardTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _userIdentifier = factory.GetUserIdentifier();
        _recipeId = factory.GetRecipeId();
    }

    [Fact]
    public async Task Success()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

        var response = await DoGet(_route, token);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        responseData.RootElement.GetProperty("recipes").GetArrayLength().ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task Success_No_Content()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

        await DoDelete(
          route: $"recipe/{_recipeId}",
          token: token
          ); 

        var response = await DoGet(_route, token);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }
}

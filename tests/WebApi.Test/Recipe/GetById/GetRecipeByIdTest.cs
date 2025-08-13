using CommonTestUtilities.IdEncripter;
using CommonTestUtilities.Tokens;
using MyRecipeBook.Exceptions;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.Recipe.GetById;

public class GetRecipeByIdTest : MyRecipeBookClassFixture
{
    private readonly string _route = "recipe",
        _recipeId,
        _recipeTitle;

    private readonly Guid _userIdentifier;

    public GetRecipeByIdTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _userIdentifier = factory.GetUserIdentifier();
        _recipeId = factory.GetRecipeId();
        _recipeTitle = factory.GetRecipeTitle();
    }

    [Fact]
    public async Task Success()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

        var response = await DoGet(
            route: $"{_route}/{_recipeId}", 
            token: token
            );

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        responseData.RootElement.GetProperty("id").GetString().ShouldBe(_recipeId);
        responseData.RootElement.GetProperty("title").GetString().ShouldBe(_recipeTitle);
    }

    [Theory]
    [ClassData(typeof(CultueInlineDataTest))]
    public async Task Error_Recipe_Not_Found(string culture)
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

        var id = IdEncripterBuilder.Build().Encode(1000);

        var response = await DoGet(
            route: $"{_route}/{id}", 
            token: token, 
            culture: culture
        );

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var errors = await GetErrosFromResponse(response);

        var expectedMessage = ResourceMessagesException.ResourceManager.GetString("RECIPE_NOT_FOUND", new CultureInfo(culture))!;

        errors.ShouldSatisfyAllConditions(
            v => v.ShouldHaveSingleItem(),
            v => v.ShouldContain(e => e.GetString()!.Equals(expectedMessage))
            );
    }
}

using CommonTestUtilities.IdEncripter;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using MyRecipeBook.Exceptions;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.Recipe.Update;

public class UpdateRecipeTest : MyRecipeBookClassFixture
{
    private readonly string _route = "recipe",
        _recipeId;

    private readonly Guid _userIdentifier;

    public UpdateRecipeTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _userIdentifier = factory.GetUserIdentifier();
        _recipeId = factory.GetRecipeId();
    }

    [Fact]
    public async Task Success()
    {
        var body = RequestRecipeJsonBuilder.Build();

        var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

        var response = await DoPut(
            route: $"{_route}/{_recipeId}",
            body: body, 
            token: token
            );

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Theory]
    [ClassData(typeof(CultueInlineDataTest))]
    public async Task Error_Title_Empty(string culture)
    {
        var body = RequestRecipeJsonBuilder.Build();
        body.Title = string.Empty;

        var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

        var response = await DoPut(
           route: $"{_route}/{_recipeId}",
           body: body,
           token: token,
           culture: culture
           );

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var errors = await GetErrosFromResponse(response);

        var expectedMessage = ResourceMessagesException.ResourceManager.GetString("RECIPE_TITLE_EMPTY", new CultureInfo(culture))!;

        errors.ShouldSatisfyAllConditions(
            v => v.ShouldHaveSingleItem(),
            v => v.ShouldContain(e => e.GetString()!.Equals(expectedMessage))
            );
    }

    [Theory]
    [ClassData(typeof(CultueInlineDataTest))]
    public async Task Error_Recipe_Not_Found(string culture)
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

        var id = IdEncripterBuilder.Build().Encode(1000);

        var body = RequestRecipeJsonBuilder.Build();

        var response = await DoPut(
           route: $"{_route}/{id}",
           body: body,
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

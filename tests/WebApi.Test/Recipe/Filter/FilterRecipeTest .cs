using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using MyRecipeBook.Communication.Requests.Recipe;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Exceptions;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Test.InlineData;
using Shouldly;

namespace WebApi.Test.Recipe.Filter;

public class FilterRecipeTest : MyRecipeBookClassFixture
{
    private readonly string _route = "recipe/filter",
        _recipeTitle;

    private readonly Guid _userIdentifier;

    private MyRecipeBook.Domain.Enums.Difficulty _recipedifficultyLevel;
    private MyRecipeBook.Domain.Enums.CookingTime _recipeCookingTime;
    private IList<MyRecipeBook.Domain.Enums.DishType> _recipeDishTypes;

    public FilterRecipeTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _userIdentifier = factory.GetUserIdentifier();

        _recipeTitle = factory.GetRecipeTitle();
        _recipeCookingTime = factory.GetRecipeCookingTime();
        _recipedifficultyLevel = factory.GetRecipeDifficulty();
        _recipeDishTypes = factory.GetDishTypes();
    }

    [Fact]
    public async Task Success()
    {
        var body = new RequestFilterRecipeJson
        {
            CookingTimes = [(CookingTime)_recipeCookingTime],
            Difficulties = [(Difficulty)_recipedifficultyLevel],
            DishTypes = _recipeDishTypes.Select(dishType => (DishType)dishType).ToList(),
            RecipeTitle_Ingredient = _recipeTitle,
        };

        var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

        var response = await DoPost(route: _route, body: body, token: token);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        responseData.RootElement.GetProperty("recipes").EnumerateArray().ShouldNotBeEmpty();
    }

    [Fact]
    public async Task Success_NoContent()
    {
        var body = RequestFilterRecipeJsonBuilder.Build();
        body.RecipeTitle_Ingredient = "recipeDontExist";

        var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

        var response = await DoPost(route: _route, body: body, token: token);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Theory]
    [ClassData(typeof(CultueInlineDataTest))]
    public async Task Error_CookingTime_Invalid(string culture)
    {
        var body = RequestFilterRecipeJsonBuilder.Build();
        body.CookingTimes.Add((CookingTime)1000);

        var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

        var response = await DoPost(
            route: _route,
            body: body,
            token: token,
            culture: culture
            );

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var errors = await GetErrosFromResponse(response);

        var expectedMessage = ResourceMessagesException.ResourceManager.GetString("COOKING_TIME_NOT_SUPPORTED", new CultureInfo(culture))!;

        errors.ShouldSatisfyAllConditions(
            v => v.ShouldHaveSingleItem(),
            v => v.ShouldContain(e => e.GetString()!.Equals(expectedMessage))
            );
    }
}

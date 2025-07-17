using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using MyRecipeBook.Exceptions;
using Shouldly;
using System.Globalization;
using System.Net;
using WebApi.Test.InlineData;

namespace WebApi.Test.Recipe.Register;

public class RegisterRecipeInvalidTokenTest(CustomWebApplicationFactory factory) : MyRecipeBookClassFixture(factory)
{
    private readonly string _route = "recipe";

    [Theory]
    [ClassData(typeof(CultueInlineDataTest))]
    public async Task Error_Token_Invalid(string culture)
    {
        var expectedMessage = ResourceMessagesException.ResourceManager.GetString("UNKNOW_ERROR", new CultureInfo(culture))!;

        await ErrorToken(token: "tokenInvalid", expectedMessage, culture);
    }

    [Theory]
    [ClassData(typeof(CultueInlineDataTest))]
    public async Task Error_Without_Token(string culture)
    {
        var expectedMessage = ResourceMessagesException.ResourceManager.GetString("NO_TOKEN", new CultureInfo(culture))!;

        await ErrorToken(token: string.Empty, expectedMessage, culture);
    }

    [Theory]
    [ClassData(typeof(CultueInlineDataTest))]
    public async Task Error_Token_With_User_NotFound(string culture)
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid());

        var expectedMessage = ResourceMessagesException.ResourceManager.GetString("USER_WITHOUT_PERMISSION_ACESS_RESOURCE", new CultureInfo(culture))!;

        await ErrorToken(token, expectedMessage, culture);
    }

    private async Task ErrorToken(string token, string expectedMessage, string culture)
    {
        var body = RequestRecipeJsonBuilder.Build();

        var response = await DoPost(
            route: _route, 
            body: body, 
            token: token, 
            culture: culture
            );

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        var errors = await GetErrosFromResponse(response);

        errors.ShouldSatisfyAllConditions(
            v => v.ShouldHaveSingleItem(),
            v => v.ShouldContain(e => e.GetString()!.Equals(expectedMessage))
            );
    }
}

using CommonTestUtilities.Requests;
using MyRecipeBook.Exceptions;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.User.Register;

public class RegisterUserTest : MyRecipeBookClassFixture
{
    private readonly string _route = "user";

    public RegisterUserTest(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Success()
    {
        var body = RequestRegisterUserJsonBuilder.Build();

        var response = await DoPost(route: _route, body: body);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        await using var resposeBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(resposeBody);

        responseData.RootElement.GetProperty("name").GetString().ShouldSatisfyAllConditions(
            v => v.ShouldBe(body.Name),
            v => v.ShouldNotBeNullOrWhiteSpace()
            );
        responseData.RootElement.GetProperty("tokens").GetProperty("accessToken").GetString().ShouldNotBeNullOrEmpty();
    }

    [Theory]
    [ClassData(typeof(CultueInlineDataTest))]
    public async Task Error_Empty_Name(string culture)
    {
        var body = RequestRegisterUserJsonBuilder.Build();
        body.Name = string.Empty;

        var response = await DoPost(route: _route, body: body, culture: culture);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        await using var resposeBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(resposeBody);

        var erros = responseData.RootElement.GetProperty("errors").EnumerateArray();

        var expectedMessage = ResourceMessagesException.ResourceManager.GetString("NAME_EMPTY", new CultureInfo(culture));

        erros.ShouldSatisfyAllConditions(
            v => v.ShouldHaveSingleItem(),
            v => v.ShouldContain(e => e.GetString()!.Equals(expectedMessage))
            );
    }
}

using CommonTestUtilities.Requests;
using MyRecipeBook.Exceptions;
using Shouldly;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using WebApi.Test.InlineData;
using System.Globalization;

namespace WebApi.Test.User.Register;

public class RegisterUserTest : MyRecipeBookClassFixture
{
    private readonly string route = "user";

    public RegisterUserTest(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Success()
    {
        var body = RequestRegisterUserJsonBuilder.Build();

        var response = await DoPost(route, body);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        await using var resposeBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(resposeBody);

        responseData.RootElement.GetProperty("name").GetString().ShouldSatisfyAllConditions(
            v => v.ShouldBe(body.Name),
            v => v.ShouldNotBeNullOrWhiteSpace()
            );
    }

    [Theory]
    [ClassData(typeof(CultueInlineDataTest))]
    public async Task Error_Empty_Name(string culture)
    {
        var body = RequestRegisterUserJsonBuilder.Build();
        body.Name = string.Empty;

        var response = await DoPost(route, body, culture);

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

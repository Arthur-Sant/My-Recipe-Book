using CommonTestUtilities.Requests;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.Login.DoLogin;

public class DoLoginTest : IClassFixture<CustomWebApplicationFactory>
{
    private readonly string route = "login",
        _email, 
        _password,
        _name;

    private readonly HttpClient _httpClient;

    public DoLoginTest(CustomWebApplicationFactory factory)
    {
        _httpClient = factory.CreateClient();

        _email = factory.GetEmail();
        _password = factory.GetPassword();
        _name = factory.GetName();
    }

    [Fact]
    public async Task Success()
    {
        var body = new RequestLoginJson {
            Email = _email,
            Password = _password
        };

        var response = await _httpClient.PostAsJsonAsync(route, body);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        await using var resposeBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(resposeBody);

        responseData.RootElement.GetProperty("name").GetString().ShouldSatisfyAllConditions(
            v => v.ShouldBe(_name),
            v => v.ShouldNotBeNullOrWhiteSpace()
            );
    }

    [Theory]
    [ClassData(typeof(CultueInlineDataTest))]
    public async Task Error_Login_Invalid(string culture)
    {
        var body = RequestLoginJsonBuilder.Build();

        if(_httpClient.DefaultRequestHeaders.Contains("Accept-Language"))
            _httpClient.DefaultRequestHeaders.Remove("Accept-Language");

        _httpClient.DefaultRequestHeaders.Add("Accept-Language", culture);

        var response = await _httpClient.PostAsJsonAsync(route, body);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        await using var resposeBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(resposeBody);

        var erros = responseData.RootElement.GetProperty("errors").EnumerateArray();

        var expectedMessage = ResourceMessagesException.ResourceManager.GetString("EMAIL_OR_PASSWORD_INVALID", new CultureInfo(culture));

        erros.ShouldSatisfyAllConditions(
            v => v.ShouldHaveSingleItem(),
            v => v.ShouldContain(e => e.GetString()!.Equals(expectedMessage))
            );
    }
}

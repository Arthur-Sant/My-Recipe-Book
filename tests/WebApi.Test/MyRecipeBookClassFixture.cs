using Microsoft.AspNetCore.Routing;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace WebApi.Test;
public class MyRecipeBookClassFixture : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _httpClient;

    public MyRecipeBookClassFixture(CustomWebApplicationFactory factory) => _httpClient = factory.CreateClient();

    protected async Task<HttpResponseMessage> DoPost(string route, object body, string culture = "en")
    {
        ChangeRequestCulture(culture);

        return await _httpClient.PostAsJsonAsync(route, body);
    }

    protected async Task<HttpResponseMessage> DoGet(string route, string token = "", string culture = "en")
    {
        ChangeRequestCulture(culture);
        AuthorizeRequest(token);

        return await _httpClient.GetAsync(route);

    }

    protected async Task<HttpResponseMessage> DoPut(string route, object body, string token, string culture = "en")
    {
        ChangeRequestCulture(culture);
        AuthorizeRequest(token);

        return await _httpClient.PutAsJsonAsync(route, body);
    }

    protected async Task<HttpResponseMessage> DoPatch(string route, object body, string token, string culture = "en")
    {
        ChangeRequestCulture(culture);
        AuthorizeRequest(token);

        return await _httpClient.PatchAsJsonAsync(route, body);
    }

    protected static async Task<JsonElement.ArrayEnumerator> GetErrosFromResponse(HttpResponseMessage response)
    {
        await using var resposeBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(resposeBody);

        return responseData.RootElement.GetProperty("errors").EnumerateArray();
    }

    private void ChangeRequestCulture(string culture)
    {
        if(_httpClient.DefaultRequestHeaders.Contains("Accept-Language"))
            _httpClient.DefaultRequestHeaders.Remove("Accept-Language");

        _httpClient.DefaultRequestHeaders.Add("Accept-Language", culture);
    }

    private void AuthorizeRequest(string token)
    {
        if(string.IsNullOrWhiteSpace(token))
            return;

        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }
}

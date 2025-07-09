using Microsoft.AspNetCore.Routing;
using System.Net.Http.Json;

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

    private void ChangeRequestCulture(string culture)
    {
        if(_httpClient.DefaultRequestHeaders.Contains("Accept-Language"))
            _httpClient.DefaultRequestHeaders.Remove("Accept-Language");

        _httpClient.DefaultRequestHeaders.Add("Accept-Language", culture);
    }
}

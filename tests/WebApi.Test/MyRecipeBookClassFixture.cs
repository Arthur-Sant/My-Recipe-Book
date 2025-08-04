using System.Net.Http.Json;
using System.Numerics;
using System.Text.Json;
using System.Collections;

namespace WebApi.Test;
public class MyRecipeBookClassFixture : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _httpClient;

    public MyRecipeBookClassFixture(CustomWebApplicationFactory factory) => _httpClient = factory.CreateClient();

    protected async Task<HttpResponseMessage> DoPost(string route, object body, string token = "", string culture = "en")
    {
        ChangeRequestCulture(culture);
        AuthorizeRequest(token);

        return await _httpClient.PostAsJsonAsync(route, body);
    }

    protected async Task<HttpResponseMessage> DoPostFormData(string route, object body, string token = "", string culture = "en")
    {
        ChangeRequestCulture(culture);
        AuthorizeRequest(token);

        var multipartContent = new MultipartFormDataContent();
        
        var bodyProperties = body.GetType().GetProperties().ToList();

        foreach(var property in bodyProperties)
        {
            var propertyValue = property.GetValue(body);

            if( string.IsNullOrWhiteSpace(propertyValue?.ToString()))
                continue;
            
            if(propertyValue is IList)
                AddListToMultipartContent(multipartContent, property.Name, (IList)propertyValue);
            else
                multipartContent.Add(new StringContent(propertyValue.ToString()!), property.Name);
        }

        return await _httpClient.PostAsync(route, multipartContent);
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

    protected async Task<HttpResponseMessage> DoDelete(string route, string token, string culture = "en")
    {
        ChangeRequestCulture(culture);
        AuthorizeRequest(token);

        return await _httpClient.DeleteAsync(route);
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

    private void AddListToMultipartContent(
        MultipartFormDataContent multipartContent, 
        string propertyName, 
        IList list
        )
    {
        var itemType = list.GetType().GetGenericArguments().Single();

        if(itemType.IsClass && itemType != typeof(string))
        {
            AddClassListToMultipartContent(multipartContent, propertyName, list);
        }
        else
        {
            foreach(var item in list)
            {
                multipartContent.Add(new StringContent(item.ToString()!), propertyName);
            }
        }

    }

    private static void AddClassListToMultipartContent(
      MultipartFormDataContent multipartContent,
      string propertyName,
      System.Collections.IList list)
    {
        var index = 0;

        foreach(var item in list)
        {
            var classPropertiesInfo = item.GetType().GetProperties().ToList();

            foreach(var prop in classPropertiesInfo)
            {
                var value = prop.GetValue(item, null);
                multipartContent.Add(new StringContent(value!.ToString()!), $"{propertyName}[{index}][{prop.Name}]");
            }

            index++;
        }
    }
}

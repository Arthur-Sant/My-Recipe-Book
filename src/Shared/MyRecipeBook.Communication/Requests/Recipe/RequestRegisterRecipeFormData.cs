using Microsoft.AspNetCore.Http;

namespace MyRecipeBook.Communication.Requests.Recipe;

public class RequestRegisterRecipeFormData : RequestRecipeJson
{
    public IFormFile? Image { get; set; }
}

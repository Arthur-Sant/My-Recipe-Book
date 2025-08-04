namespace MyRecipeBook.Communication.Responses.Recipe;

public class ResponseShortRecipeJson
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int AmountIngredients { get; set; }
    public string? ImageUrl { get; set; } = string.Empty;
}

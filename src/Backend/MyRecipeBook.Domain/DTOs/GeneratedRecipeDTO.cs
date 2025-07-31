using MyRecipeBook.Domain.Enums;

namespace MyRecipeBook.Domain.DTOs;

public class GeneratedRecipeDTO
{
    public string Title { get; init; } = string.Empty;
    public IList<string> Ingredients { get; init; } = [];
    public IList<GeneratedInstructionDTO> Instructions { get; init; } = [];
    public CookingTime CookingTime { get; init; }
}

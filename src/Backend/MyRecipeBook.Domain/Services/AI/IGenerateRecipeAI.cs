using MyRecipeBook.Domain.DTOs;

namespace MyRecipeBook.Domain.Services.AI;

public interface IGenerateRecipeAI
{
    public Task<GeneratedRecipeDTO> Generate(IList<string> ingredients);
}

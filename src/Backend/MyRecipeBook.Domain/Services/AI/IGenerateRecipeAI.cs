using MyRecipeBook.Domain.DTOs;

namespace MyRecipeBook.Domain.Services.AI;

public interface IGenerateRecipeAI
{
    public Task<GeneratedRecipeDto> Generate(IList<string> ingredients);
}

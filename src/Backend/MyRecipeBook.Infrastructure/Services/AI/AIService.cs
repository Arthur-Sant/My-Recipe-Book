using Mscc.GenerativeAI;
using MyRecipeBook.Domain.DTOs;
using MyRecipeBook.Domain.Enums;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Services.AI;
using MyRecipeBook.Infrastructure.Services.AI;

namespace MyRecipeBook.Infrastructure.Services.OpenAI;

public class AIService(IGenerativeAI _googleAi) : IGenerateRecipeAI
{
    public async Task<GeneratedRecipeDTO> Generate(IList<string> ingredients)
    {
        var model = _googleAi.GenerativeModel(model: Model.Gemma3n_E2B);

        var input = ResourceAI.STARTING_GENERATE_RECIPE.Replace("{}", string.Join(";", ingredients));

        var response = await model.GenerateContent(prompt: input);

        var responseList = response.Text
          .Split("\n")
          .Where(response => response.Trim().Equals(string.Empty).IsFalse())
          .Select(item => item.Replace("[", "").Replace("]", ""))
          .ToList();

        var step = 1;

        return new GeneratedRecipeDTO
        {
            Title = responseList[0],
            CookingTime = (CookingTime)Enum.Parse(typeof(CookingTime), responseList[1].First().ToString()),
            Ingredients = responseList[2].Split(";"),
            Instructions = responseList[3].Split("@").Select(instruction => new GeneratedInstructionDTO
            {
                Text = instruction.Trim(),
                Step = step++
            }).ToList()
        };
    }
}

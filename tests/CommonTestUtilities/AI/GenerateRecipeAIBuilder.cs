using Moq;
using MyRecipeBook.Domain.DTOs;
using MyRecipeBook.Domain.Services.AI;

namespace CommonTestUtilities.AI;

public class GenerateRecipeAIBuilder
{
    public static IGenerateRecipeAI Build(GeneratedRecipeDTO dto)
    {
        var mock = new Mock<IGenerateRecipeAI>();

        mock.Setup(service => service.Generate(It.IsAny<List<string>>())).ReturnsAsync(dto);

        return mock.Object;
    }
}

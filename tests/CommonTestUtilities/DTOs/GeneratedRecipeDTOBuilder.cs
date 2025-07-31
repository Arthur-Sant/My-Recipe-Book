using Bogus;
using MyRecipeBook.Domain.DTOs;
using MyRecipeBook.Domain.Enums;

namespace CommonTestUtilities.DTOs;

public class GeneratedRecipeDTOBuilder
{
    public static GeneratedRecipeDTO Build()
    {
        return new Faker<GeneratedRecipeDTO>()
            .RuleFor(r => r.Title, faker => faker.Lorem.Word())
            .RuleFor(r => r.CookingTime, faker => faker.PickRandom<CookingTime>())
            .RuleFor(r => r.Ingredients, faker => faker.Make(1, () => faker.Commerce.ProductName()))
            .RuleFor(r => r.Instructions, faker => faker.Make(1, () => new GeneratedInstructionDTO
            {
                Step = 1,
                Text = faker.Lorem.Paragraph()
            }));
    }
}

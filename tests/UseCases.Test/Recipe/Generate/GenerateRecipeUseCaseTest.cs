using CommonTestUtilities.AI;
using CommonTestUtilities.DTOs;
using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.Recipe.Generate;
using MyRecipeBook.Domain.DTOs;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;

namespace UseCases.Test.Recipe.Generate;

public class GenerateRecipeUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var dto = GeneratedRecipeDtoBuilder.Build();

        var body = RequestGenerateRecipeJsonBuilder.Build();

        var useCase = CreateUseCase(dto);

        var result = await useCase.Execute(body);

        result.ShouldNotBeNull();
        result.Title.ShouldBe(dto.Title);
        result.CookingTime.ShouldBe((MyRecipeBook.Communication.Enums.CookingTime)dto.CookingTime);
        result.Difficulty.ShouldBe(MyRecipeBook.Communication.Enums.Difficulty.Low);
    }

    [Fact]
    public async Task Error_Duplicated_Ingredients()
    {
        var dto = GeneratedRecipeDtoBuilder.Build();

        var request = RequestGenerateRecipeJsonBuilder.Build(count: 4);
        request.Ingredients.Add(request.Ingredients[0]);

        var useCase = CreateUseCase(dto);

        var act = async () => await useCase.Execute(request);

        (await act.ShouldThrowAsync<ErrorOnValidationException>())
          .ShouldSatisfyAllConditions(
          e => e.GetErrorMessages().ShouldHaveSingleItem(),
          e => e.GetErrorMessages().ShouldContain(m => m.Equals(ResourceMessagesException.DUPLICATED_INGREDIENTS_IN_LIST))
          );
    }

    private static GenerateRecipeUseCase CreateUseCase(GeneratedRecipeDto dto)
    {
        var generateRecipeAI = GenerateRecipeAIBuilder.Build(dto);

        return new GenerateRecipeUseCase(generateRecipeAI);
    }
}

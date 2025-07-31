using MyRecipeBook.Communication.Requests.Recipe;
using MyRecipeBook.Communication.Responses.Recipe;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Services.AI;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.Generate;

public class GenerateRecipeUseCase(IGenerateRecipeAI _generator) : IGenerateRecipeUseCase
{
    public async Task<ResponseGeneratedRecipeJson> Execute(RequestGenerateRecipeJson body)
    {
        Validate(body);

        var response = await _generator.Generate(body.Ingredients);

        return new ResponseGeneratedRecipeJson
        {
            Title = response.Title,
            Ingredients = response.Ingredients,
            CookingTime = (Communication.Enums.CookingTime)response.CookingTime,
            Instructions = response.Instructions.Select(c => new ResponseGeneratedInstructionJson
            {
                Step = c.Step,
                Text = c.Text,
            }).ToList(),
            Difficulty = Communication.Enums.Difficulty.Low
        };
    }

    private static void Validate(RequestGenerateRecipeJson body)
    {
        var result = new GenerateRecipeValidator().Validate(body);

        if(result.IsValid.IsFalse())
            throw new ErrorOnValidationException(result.Errors.Select(e => e.ErrorMessage).ToList());
    }
}

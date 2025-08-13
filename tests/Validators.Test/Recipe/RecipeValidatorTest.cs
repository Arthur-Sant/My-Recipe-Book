using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.Recipe;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Exceptions;
using Shouldly;
using System.Diagnostics.CodeAnalysis;

namespace Validators.Test.Recipe;

public class RecipeValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new RecipeValidator();

        var body = RequestRecipeJsonBuilder.Build();

        var result = validator.Validate(body);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Error_Invalid_Cooking_Time()
    {
        var validator = new RecipeValidator();

        var body = RequestRecipeJsonBuilder.Build();
        body.CookingTime = (MyRecipeBook.Communication.Enums.CookingTime?)1000;

        var result = validator.Validate(body);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
            e => e.ShouldHaveSingleItem(),
            e => e.ShouldContain(m => m.ErrorMessage.Equals(ResourceMessagesException.COOKING_TIME_NOT_SUPPORTED))
            );
    }

    [Fact]
    public void Error_Invalid_Difficulty()
    {
        var validator = new RecipeValidator();

        var body = RequestRecipeJsonBuilder.Build();
        body.Difficulty = (MyRecipeBook.Communication.Enums.Difficulty?)1000;

        var result = validator.Validate(body);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
            e => e.ShouldHaveSingleItem(),
            e => e.ShouldContain(m => m.ErrorMessage.Equals(ResourceMessagesException.DIFFICULTY_LEVEL_NOT_SUPPORTED))
            );
    }

    [Theory]
    [InlineData(null)]
    [InlineData("       ")]
    [InlineData("")]
    public void Error_Empty_Title(string title)
    {
        var validator = new RecipeValidator();

        var body = RequestRecipeJsonBuilder.Build();
        body.Title = title;

        var result = validator.Validate(body);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
            e => e.ShouldHaveSingleItem(),
            e => e.ShouldContain(m => m.ErrorMessage.Equals(ResourceMessagesException.RECIPE_TITLE_EMPTY))
            );
    }

    [Fact]
    public void Success_Cokking_Time_Null()
    {
        var validator = new RecipeValidator();

        var body = RequestRecipeJsonBuilder.Build();
        body.CookingTime = null;

        var result = validator.Validate(body);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Success_Difficulty_Null()
    {
        var validator = new RecipeValidator();

        var body = RequestRecipeJsonBuilder.Build();
        body.Difficulty = null;

        var result = validator.Validate(body);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Success_DishTypes_Empty()
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.DishTypes.Clear();

        var validator = new RecipeValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Error_Invalid_DishTypes()
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.DishTypes.Add((DishType)1000);

        var validator = new RecipeValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
           e => e.ShouldHaveSingleItem(),
           e => e.ShouldContain(m => m.ErrorMessage.Equals(ResourceMessagesException.DISH_TYPE_NOT_SUPPORTED))
           );
    }

    [Fact]
    public void Error_Empty_Ingredients()
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.Ingredients.Clear();

        var validator = new RecipeValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
           e => e.ShouldHaveSingleItem(),
           e => e.ShouldContain(m => m.ErrorMessage.Equals(ResourceMessagesException.AT_LEAST_ONE_INGREDIENT))
           );
    }

    [Fact]
    public void Error_Empty_Instructions()
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.Instructions.Clear();

        var validator = new RecipeValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
           e => e.ShouldHaveSingleItem(),
           e => e.ShouldContain(m => m.ErrorMessage.Equals(ResourceMessagesException.AT_LEAST_ONE_INSTRUCTION))
           );
    }

    [Theory]
    [InlineData("   ")]
    [InlineData("")]
    [InlineData(null)]
    public void Error_Empty_Value_Ingredients(string ingredient)
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.Ingredients.Add(ingredient);

        var validator = new RecipeValidator();

        var result = validator.Validate(request);


        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
           e => e.ShouldHaveSingleItem(),
           e => e.ShouldContain(m => m.ErrorMessage.Equals(ResourceMessagesException.INGREDIENT_EMPTY))
           );
    }

    [Fact]
    public void Error_Same_Step_Instructions()
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.Instructions.First().Step = request.Instructions.Last().Step;

        var validator = new RecipeValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
           e => e.ShouldHaveSingleItem(),
           e => e.ShouldContain(m => m.ErrorMessage.Equals(ResourceMessagesException.TWO_OR_MORE_INSTRUCTIONS_SAME_ORDER))
           );
    }

    [Fact]
    public void Error_Negative_Step_Instructions()
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.Instructions.First().Step = -1;

        var validator = new RecipeValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
           e => e.ShouldHaveSingleItem(),
           e => e.ShouldContain(m => m.ErrorMessage.Equals(ResourceMessagesException.NON_NEGATIVE_INSTRUCTION_STEP))
           );
    }

    [Theory]
    [InlineData("   ")]
    [InlineData("")]
    [InlineData(null)]
    public void Error_Empty_Value_Instructions(string instruction)
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.Instructions.First().Text = instruction;

        var validator = new RecipeValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
           e => e.ShouldHaveSingleItem(),
           e => e.ShouldContain(m => m.ErrorMessage.Equals(ResourceMessagesException.INSTRUCTION_EMPTY))
           );
    }

    [Fact]
    public void Error_Instructions_Too_Long()
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.Instructions.First().Text = RequestStringGenerator.Paragraphs(minCharacters: 2001);

        var validator = new RecipeValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
           e => e.ShouldHaveSingleItem(),
           e => e.ShouldContain(m => m.ErrorMessage.Equals(ResourceMessagesException.INSTRUCTION_EXCEEDS_LIMIT_CHARACTERS))
           );
    }
}

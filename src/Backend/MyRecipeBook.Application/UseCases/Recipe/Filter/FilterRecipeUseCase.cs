using AutoMapper;
using MyRecipeBook.Communication.Requests.Recipe;
using MyRecipeBook.Communication.Responses.Recipe;
using MyRecipeBook.Domain.DTOs;
using MyRecipeBook.Domain.Enums;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.Filter;

public class FilterRecipeUseCase(
    IMapper _mapper,
    ILoggedUser _loggedUser,
    IRecipeReadOnlyRepository _repository
    ) : IFilterRecipeUseCase
{
    public async Task<ResponseRecipesJson> Execute(RequestFilterRecipeJson body)
    {
        Validate(body);

        var loggedUser = await _loggedUser.User();

        var filters = new FilterRecipesDto
        {
            RecipeTitle_Ingredient = body.RecipeTitle_Ingredient,
            CookingTimes = body.CookingTimes.Distinct().Select(c => (CookingTime)c).ToList(),
            Difficulties = body.Difficulties.Distinct().Select(c => (Difficulty)c).ToList(),
            DishTypes = body.DishTypes.Distinct().Select(c => (DishType)c).ToList()
        };

        var recipes = await _repository.Filter(loggedUser, filters);

        return new ResponseRecipesJson
        {
            Recipes = _mapper.Map<List<ResponseShortRecipeJson>>(recipes)
        };
    }

    private static void Validate(RequestFilterRecipeJson request)
    {
        var validator = new FilterRecipeValidator();

        var result = validator.Validate(request);

        if(result.IsValid.IsFalse())
        {
            var errorMessages = result.Errors.Select(error => error.ErrorMessage).Distinct().ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}

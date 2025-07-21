using AutoMapper;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.Update;

public class UpdateRecipeUseCase
    (
    ILoggedUser _loggedUser,
    IRecipeUpdateOnlyRepository _repository,
    IUnityOfWork _unitOfWork,
    IMapper _mapper
    ) : IUpdateRecipeUseCase
{
    public async Task Execute(long recipeId, RequestRecipeJson body)
    {
        Validate(body);

        var loggedUser = await _loggedUser.User();

        var recipe = await _repository.GetById(recipeId, loggedUser.Id);

        if(recipe is null)
            throw new NotFoundException(ResourceMessagesException.RECIPE_NOT_FOUND);

        recipe.Ingredients.Clear();
        recipe.Instructions.Clear();
        recipe.DishTypes.Clear();

        _mapper.Map(body, recipe);

        var instructions = body.Instructions.OrderBy(i => i.Step).ToList();
        for(var index = 0; index < instructions.Count; index++)
            instructions[index].Step = index + 1;

        recipe.Instructions = _mapper.Map<IList<Domain.Entities.Instruction>>(instructions);

        _repository.Update(recipe);

        await _unitOfWork.Commit();
    }

    private static void Validate(RequestRecipeJson body)
    {
        var result = new RecipeValidator().Validate(body);

        if(result.IsValid.IsFalse())
            throw new ErrorOnValidationException(result.Errors.Select(e => e.ErrorMessage).Distinct().ToList());
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MyRecipeBook.Domain.DTOs;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repositories.Recipe;

namespace MyRecipeBook.Infrastructure.DataAccess.Repositories;

public sealed class RecipeRepository(MyRecipeBookDbContext _dbContext) 
    : IRecipeWriteOnlyRepository, IRecipeReadOnlyRepository, IRecipeDeleteOnlyRepository, IRecipeUpdateOnlyRepository
{
    public async Task Add(Recipe recipe) => await _dbContext.Recipes.AddAsync(recipe);

    public void Delete(long id)
    {
        _dbContext.Recipes.Remove(new Recipe { Id = id });
    }

    public async Task<IList<Recipe>> Filter(User user, FilterRecipesDTO filters)
    {
        var query = _dbContext.Recipes
            .AsNoTracking()
            .Include(recipe => recipe.Ingredients)
            .Where(recipe => recipe.Active && recipe.UserId == user.Id);

        if(filters.Difficulties.Any())
        {
            query = query.Where(recipe => recipe.Difficulty.HasValue && filters.Difficulties.Contains(recipe.Difficulty.Value));
        }

        if(filters.CookingTimes.Any())
        {
            query = query.Where(recipe => recipe.CookingTime.HasValue && filters.CookingTimes.Contains(recipe.CookingTime.Value));
        }

        if(filters.DishTypes.Any())
        {
            query = query.Where(recipe => recipe.DishTypes.Any(dishType => filters.DishTypes.Contains(dishType.Type))); 
        }

        if(filters.RecipeTitle_Ingredient.NotEmpty())
        {
            query = query.Where(recipe => 
                recipe.Title.Contains(filters.RecipeTitle_Ingredient)
                || recipe.Ingredients.Any(ingredient => ingredient.Item.Contains(filters.RecipeTitle_Ingredient))
            );
        }

        return await query.ToListAsync();
    }

    public async Task<Recipe?> GetById(User user, long id)
    {
        return await GetFullRecipe()
            .AsNoTracking()
            .FirstOrDefaultAsync(recipe => recipe.Active && recipe.Id.Equals(id) && recipe.UserId.Equals(user.Id));
    }

    public async Task<Recipe?> GetById(long id, long userId)
    {
        return await GetFullRecipe()
            .FirstOrDefaultAsync(recipe => recipe.Active && recipe.Id.Equals(id) && recipe.UserId.Equals(userId));
    }

    public async Task<bool> RecipeExists(long id, long userId)
    {
        return await _dbContext.Recipes.AnyAsync(recipe => recipe.Id.Equals(id) && recipe.UserId.Equals(userId));
    }

    public void Update(Recipe recipe) => _dbContext.Recipes.Update(recipe);

    private IIncludableQueryable<Recipe, IList<DishType>> GetFullRecipe()
    {
        return _dbContext
            .Recipes
            .Include(recipe => recipe.Ingredients)
            .Include(recipe => recipe.Instructions)
            .Include(recipe => recipe.DishTypes);
    }
}

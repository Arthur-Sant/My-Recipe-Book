using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories.Recipe;

namespace MyRecipeBook.Infrastructure.DataAccess.Repositories;

public sealed class RecipeRepository(MyRecipeBookDbContext _dbContext) : IRecipeWriteOnlyRepository
{
    public async Task Add(Recipe recipe) => await _dbContext.Recipes.AddAsync(recipe);
}

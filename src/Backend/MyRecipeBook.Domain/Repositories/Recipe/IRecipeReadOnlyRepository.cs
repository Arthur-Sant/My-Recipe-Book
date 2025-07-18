using MyRecipeBook.Domain.DTOs;

namespace MyRecipeBook.Domain.Repositories.Recipe;

public interface IRecipeReadOnlyRepository
{
    public Task<IList<Entities.Recipe>> Filter(Entities.User user, FilterRecipesDTO filters);
    public Task<Entities.Recipe?> GetById(Entities.User user, long id);
    public Task<bool> RecipeExists(long id, long userId);
}

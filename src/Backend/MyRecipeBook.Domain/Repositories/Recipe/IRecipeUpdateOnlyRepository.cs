namespace MyRecipeBook.Domain.Repositories.Recipe;

public interface IRecipeUpdateOnlyRepository
{
    public Task<Entities.Recipe?> GetById(long id, long userId);
    public void Update(Entities.Recipe recipe);
}

namespace MyRecipeBook.Domain.Repositories.Recipe;

public interface IRecipeDeleteOnlyRepository
{
    public void Delete(long id);
}

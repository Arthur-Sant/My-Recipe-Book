namespace MyRecipeBook.Domain.Repositories.Token;

public interface ITokenDeleteOnlyRepository
{
    public Task DeleteAllByUserId(long userId);
}

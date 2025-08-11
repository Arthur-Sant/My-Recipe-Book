namespace MyRecipeBook.Domain.Repositories.Token;

public interface ITokenReadOnlyRepository
{
    public Task<Entities.Token?> Get(string refreshToken);
}

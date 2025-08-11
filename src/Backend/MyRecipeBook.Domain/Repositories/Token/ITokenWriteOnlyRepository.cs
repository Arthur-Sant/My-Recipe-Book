namespace MyRecipeBook.Domain.Repositories.Token;

public interface ITokenWriteOnlyRepository
{
    public Task Register(Entities.Token refreshToken);
}

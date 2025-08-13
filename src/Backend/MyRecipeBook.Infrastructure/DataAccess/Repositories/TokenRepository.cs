using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories.Token;

namespace MyRecipeBook.Infrastructure.DataAccess.Repositories;

public class TokenRepository(
     MyRecipeBookDbContext _dbContext
    ) : ITokenReadOnlyRepository, ITokenWriteOnlyRepository, ITokenDeleteOnlyRepository
{
    public async Task DeleteAllByUserId(long userId)
    {
        await _dbContext.Tokens
            .Where(t => t.UserId.Equals(userId))
            .ExecuteDeleteAsync();
    }

    public async Task<Token?> Get(string refreshToken)
    {
        return await _dbContext.Tokens
            .AsNoTracking()
            .Include(token => token.User)
            .FirstOrDefaultAsync(token => token.Value.Equals(refreshToken));
    }

    public async Task Register(Token refreshToken) => await _dbContext.Tokens.AddAsync(refreshToken);
}

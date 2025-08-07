using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories.CodeToPerformAction;

namespace MyRecipeBook.Infrastructure.DataAccess.Repositories;

public class CodeToPerformActionRepository(
    MyRecipeBookDbContext _dbContext
    ) : ICodeToPerformActionReadOnlyRepository, ICodeToPerformActionWriteOnlyRepository, ICodeToPerformActionDeleteOnlyRepository
{
    public async Task Add(CodeToPerformAction codeToPerformAction) => await _dbContext.CodeToPerformActions.AddAsync(codeToPerformAction);

    public async Task DeleteAllUserCodes(long userId)
    {
        await _dbContext.CodeToPerformActions
            .Where(c => c.UserId.Equals(userId))
            .ExecuteDeleteAsync();
    }

    public async Task<bool> ExistCode(string code) => await _dbContext.CodeToPerformActions.AnyAsync(c => c.Value.Equals(code));

    public async Task<CodeToPerformAction?> GetByCode(string code)
    {
        return await _dbContext.CodeToPerformActions
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Value.Equals(code));
    }
}

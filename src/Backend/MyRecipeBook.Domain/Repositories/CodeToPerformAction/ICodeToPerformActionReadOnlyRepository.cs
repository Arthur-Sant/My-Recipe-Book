namespace MyRecipeBook.Domain.Repositories.CodeToPerformAction;

public interface ICodeToPerformActionReadOnlyRepository
{
    public Task<Entities.CodeToPerformAction?> GetByCode(string code);
    public Task<bool> ExistCode(string code);
}

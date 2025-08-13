namespace MyRecipeBook.Domain.Repositories.CodeToPerformAction;

public interface ICodeToPerformActionWriteOnlyRepository
{
    public Task Add(Entities.CodeToPerformAction codeToPerformAction);
}

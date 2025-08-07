namespace MyRecipeBook.Domain.Repositories.CodeToPerformAction;

public interface ICodeToPerformActionDeleteOnlyRepository
{
    public Task DeleteAllUserCodes(long userId);
}

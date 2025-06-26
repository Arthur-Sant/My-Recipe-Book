using Moq;
using MyRecipeBook.Domain.Repositories;

namespace CommonTestUtilities.Repositories;
public class UnitOfWorkBuilder
{
    public static IUnityOfWork Build() => new Mock<IUnityOfWork>().Object;
}

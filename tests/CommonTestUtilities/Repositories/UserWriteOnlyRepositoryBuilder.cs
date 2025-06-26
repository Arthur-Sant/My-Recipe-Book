using Moq;
using MyRecipeBook.Domain.Repositories.User;

namespace CommonTestUtilities.Repositories;
public class UserWriteOnlyRepositoryBuilder
{
    public static IUserWriteOnlyRepository Build() => new Mock<IUserWriteOnlyRepository>().Object;

}

using Moq;
using MyRecipeBook.Domain.Repositories.Token;

namespace CommonTestUtilities.Repositories;

public class TokenWriteOnlyRepositoryBuilder
{
    public static ITokenWriteOnlyRepository Build() => new Mock<ITokenWriteOnlyRepository>().Object;
}

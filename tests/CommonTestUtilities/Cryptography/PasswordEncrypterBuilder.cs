using MyRecipeBook.Domain.Security.Cryptography;
using MyRecipeBook.Infrastructure.Security.Cryptography;

namespace CommonTestUtilities.Cryptography;

public class PasswordEncrypterBuilder
{
    public static IPasswordEncripter Build() => new PasswordEncripter();
}

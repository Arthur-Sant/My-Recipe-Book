using MyRecipeBook.Application.UseCases.Token.Refresh;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Infrastructure.Security.Tokens.Acess.Generator;
using MyRecipeBook.Infrastructure.Security.Tokens.Refresh;

namespace CommonTestUtilities.Tokens;

public class RefreshTokenGeneratorBuilder
{
    public static IRefreshTokenGenerator Build() => new RefreshTokenGenerator();

}

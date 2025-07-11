using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Infrastructure.Security.Tokens.Acess.Generator;

namespace CommonTestUtilities.Tokens;
public class JwtTokenGeneratorBuilder
{
    public static IAcessTokenGenerator Build() => new JwtTokenGenerator(_expirationTimeMinutes: 5, _signinKey: "pISzx3S35XMFOL9MENrgoBmfrw93PLXRv");
}

using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Infrastructure.Security.Tokens.Acess.Generator;

namespace CommonTestUtilities.Tokens;
public class JwtTokenGeneratorBuilder
{
    public static IAcessTokenGenerator Build() => new JwtTokenGenerator(_expirationTimeMinutes: 5, _signinKey: "ils3mrIRDd2y6eB0f28FS0681n00KPr5t");
}

using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MyRecipeBook.Infrastructure.Security.Tokens.Acess;
public abstract class JwtTokenHandler
{
    protected static SymmetricSecurityKey SecurityKey(string signinKey)
    {
        var keyInBytes = Encoding.UTF8.GetBytes(signinKey);

        return new SymmetricSecurityKey(keyInBytes);
    }
}

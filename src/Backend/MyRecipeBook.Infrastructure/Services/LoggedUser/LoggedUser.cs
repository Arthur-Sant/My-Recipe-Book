using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Infrastructure.DataAccess;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MyRecipeBook.Infrastructure.Services.LoggedUser;
public class LoggedUser(
    MyRecipeBookDbContext _dbContext,
    ITokenProvider _tokenProvider
    ) : ILoggedUser
{
    public async Task<User> User()
    {
        var token = _tokenProvider.Value();

        var jwtSecurityToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

        var identifier =  jwtSecurityToken.Claims.First(c => c.Type == ClaimTypes.Sid).Value;

        var userIdentifier = Guid.Parse(identifier);

        return await _dbContext
            .Users
            .AsNoTracking()
            .FirstAsync(user => user.Active && user.UserIdentifier == userIdentifier);
    }
}

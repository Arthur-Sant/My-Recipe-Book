using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.API.Filters;

public class AuthenticatedUserFilter(
    IAccessTokenValidator _accessTokenValidator,
    IUserReadOnlyRepository _repository
    ) : IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        try
        {
            var token = TokenOnRequest(context);

            var userIdentifier = _accessTokenValidator.ValidateAndGetUserIdentifier(token);
            var exists = await _repository.ExistActiveUserWithIdentifier(userIdentifier);

            if(exists.IsFalse())
            {
                throw new MyRecipeBookException(ResourceMessagesException.USER_WITHOUT_PERMISSION_ACESS_RESOURCE);
            }

        }
        catch(MyRecipeBookException exception)
        {
            context.Result = new UnauthorizedObjectResult(new ResponseErrorJson(exception.Message));
        }
        catch(SecurityTokenException)
        {
            context.Result = new UnauthorizedObjectResult(new ResponseErrorJson("Token Is Expired")
            {
                TokenIsExpired = true
            });
        }
        catch
        {
            context.Result = new UnauthorizedObjectResult(new ResponseErrorJson(ResourceMessagesException.UNKNOW_ERROR));
        }

    }

    private static string TokenOnRequest(AuthorizationFilterContext context)
    {
        var authentication = context.HttpContext.Request.Headers.Authorization.ToString();

        if(authentication.NotEmpty())
        {
            throw new MyRecipeBookException(ResourceMessagesException.NO_TOKEN);
        }

        return authentication["Bearer ".Length..].Trim();
    }
}

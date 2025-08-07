using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using MyRecipeBook.Application.UseCases.Login.DoLogin;
using MyRecipeBook.Application.UseCases.Login.External;
using MyRecipeBook.Application.UseCases.Login.RequestCode;
using MyRecipeBook.Application.UseCases.Login.ResetPassword;
using MyRecipeBook.Communication.Requests.Login;
using MyRecipeBook.Communication.Responses.Error;
using MyRecipeBook.Communication.Responses.User;
using MyRecipeBook.Exceptions;
using System.Security.Claims;


namespace MyRecipeBook.API.Controllers;

public class LoginController : MyRecipeBookBaseController
{
    private readonly string[] allowedUrls = ["/dashboard", "/perfil", "/app"];

    [HttpPost]
    [ProducesResponseType(typeof(ResponseRegisterUserJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromServices] IDoLoginUseCase useCase, 
        [FromBody] RequestLoginJson body
    )
    {
        var response = await useCase.Execute(body);

        return Ok(response);
    }

    [HttpGet("google")]
    public async Task<IActionResult> LoginGoogle(
        string returnUrl,
        [FromServices] IExternalLoginUseCase useCase
        )
    {
        var authenticate = await Request.HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

        if(IsNotAuthenticated(authenticate))
        {
            return Challenge(GoogleDefaults.AuthenticationScheme);
        }
        else
        {
            if(allowedUrls.Contains(returnUrl))
            {
            
                var claims = authenticate.Principal!.Identities.First().Claims;
            
                var name = claims.First(c => c.Type == ClaimTypes.Name).Value;            
                var email = claims.First(c => c.Type == ClaimTypes.Email).Value;

                var token = await useCase.Execute(name, email);
            
                return Redirect($"{returnUrl}/{token}");
            }
                
            return BadRequest(ResourceMessagesException.INVALID_URL);
        }
    }

    [HttpGet("code-reset-password/{email}")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> RequestCodeResetPassword(
        [FromServices] IRequestCodeResetPasswordUseCase useCase,
        [FromRoute] string email
        )
    {
        await useCase.Execute(email);

        return Accepted();
    }

    [HttpPut("reset-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErrorJson),StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword(
        [FromServices] IResetPasswordUseCase useCase, 
        [FromBody] RequestResetPasswordJson body
        )
    {
        await useCase.Execute(body);

        return NoContent();
    }
}

using Microsoft.AspNetCore.Mvc;
using MyRecipeBook.Application.UseCases.Login.DoLogin;
using MyRecipeBook.Communication.Requests.Login;
using MyRecipeBook.Communication.Responses.Error;
using MyRecipeBook.Communication.Responses.User;

namespace MyRecipeBook.API.Controllers;

public class LoginController : MyRecipeBookBaseController
{
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

}

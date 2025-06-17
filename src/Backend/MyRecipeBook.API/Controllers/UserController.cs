using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyRecipeBook.Application.UseCases.User.Register;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;

namespace MyRecipeBook.API.Controllers;

[Route("[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ResponseRegisterUserJson), StatusCodes.Status201Created)]
    public async Task<IActionResult> Register(
        [FromServices] IRegisterUseCase registerUseCase,
        [FromBody] RequestRegisterUserJson body
        )
    {
        var result = await registerUseCase.Execute(body);

        return Created(string.Empty, result);
    }
}

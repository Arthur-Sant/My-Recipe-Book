using Microsoft.AspNetCore.Mvc;
using MyRecipeBook.Application.UseCases.Token.Refresh;
using MyRecipeBook.Communication.Requests.Token;
using MyRecipeBook.Communication.Responses.Token;

namespace MyRecipeBook.API.Controllers;
public class TokenController : Controller
{
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(ResponseTokensJson), StatusCodes.Status200OK)]
    public async Task<IActionResult> RefreshToken(
        [FromServices] IRefreshTokenUseCase useCase,
        [FromBody] RequestNewTokenJson body
    )
    {
        var response = await useCase.Execute(body);
        return Ok(response);
    }
}

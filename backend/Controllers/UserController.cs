using Microsoft.AspNetCore.Mvc;
using PlaylistRecommenderAPI.Models;

[ApiController]
[Route("api/usersearch")]
public class UserController : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login([FromBody] UserLogin userLogin)
    {
        Console.WriteLine($"Recebido: {userLogin?.Email}");

        if (string.IsNullOrWhiteSpace(userLogin?.Email))
        {
            Console.WriteLine("Erro: Email vazio ou nulo.");
            return BadRequest("O email é obrigatório.");
        }

        return Ok(new ApiResponse { Message = "Login registrado com sucesso." });
    }

    [HttpPost("search")]
    public IActionResult Search([FromBody] SearchRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest("Dados inválidos.");
        }

        return Ok(new { message = "Pesquisa salva com sucesso." });
    }
}
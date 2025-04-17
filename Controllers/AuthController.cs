using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly CarteiraDbContext _context;
    private readonly IConfiguration _config;
    public AuthController(CarteiraDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var user = _context.Usuarios.SingleOrDefault(u => u.Email == request.Email);
        if (user == null || !AuthHelper.VerifyPassword(request.Senha, user.Senha))
            return Unauthorized();

        var token = TokenService.GenerateToken(user, _config);
        return Ok(new { Token = token });
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        var user = new Usuario { Nome = request.Nome, Email = request.Email, Senha = AuthHelper.HashPassword(request.Senha) };
        _context.Usuarios.Add(user);
        _context.SaveChanges();
        return Ok();
    }
}
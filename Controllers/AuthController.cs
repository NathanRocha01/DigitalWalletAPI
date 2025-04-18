using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly WalletDbContext _context;
    private readonly IConfiguration _config;
    public AuthController(WalletDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var user = _context.Users.SingleOrDefault(u => u.Email == request.Email);
        if (user == null || !AuthHelper.VerifyPassword(request.Password, user.Password))
            return Unauthorized();

        var token = TokenService.GenerateToken(user, _config);
        return Ok(new { Token = token });
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        var user = new User { Name = request.Name, Email = request.Email, Password = AuthHelper.HashPassword(request.Password) };
        _context.Users.Add(user);
        _context.SaveChanges();
        return Ok();
    }
}
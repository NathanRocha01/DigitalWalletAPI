using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CarteiraController : ControllerBase
{

    private readonly CarteiraDbContext _context;
    public CarteiraController(CarteiraDbContext context)
    {
        _context = context;
    }

    [HttpGet("carteira/saldo")]
    public IActionResult ConsultarSaldo()
    {
        var userId = GetUserIdFromToken();
        var carteira = _context.Carteiras.SingleOrDefault(c => c.UsuarioId == userId);
        return Ok(new { Saldo = carteira.Saldo });
    }

    [HttpPost("carteira/adicionar")]
    public IActionResult AdicionarSaldo([FromBody] AdicionarSaldoRequest request)
    {
        var userId = GetUserIdFromToken();
        var carteira = _context.Carteiras.SingleOrDefault(c => c.UsuarioId == userId);
        carteira.Saldo += request.Valor;
        _context.SaveChanges();
        return Ok();
    }

}


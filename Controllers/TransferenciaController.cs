using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransferenciaController : ControllerBase
{
    private readonly CarteiraDbContext _context;
    public TransferenciaController(CarteiraDbContext context)
    {
        _context = context;
    }
    public IActionResult CriarTransferencia([FromBody] TransferenciaRequest request)
    {
        var origemId = GetUserIdFromToken();
        var carteiraOrigem = _context.Carteiras.SingleOrDefault(c => c.UsuarioId == origemId);
        var carteiraDestino = _context.Carteiras.SingleOrDefault(c => c.UsuarioId == request.DestinoId);

        if (carteiraOrigem.Saldo >= request.Valor)
        {
            carteiraOrigem.Saldo -= request.Valor;
            carteiraDestino.Saldo += request.Valor;

            _context.Transferencias.Add(new Transferencia { CarteiraOrigemId = origemId, CarteiraDestinoId = request.DestinoId, Valor = request.Valor, Data = DateTime.Now });
            _context.SaveChanges();

            return Ok();
        }

        return BadRequest("Saldo insuficiente");
    }

    [Authorize]
    [HttpGet("transferencias")]
    public IActionResult ListarTransferencias(DateTime? inicio, DateTime? fim)
    {
        var userId = GetUserIdFromToken();
        var transferencias = _context.Transferencias
            .Where(t => t.CarteiraOrigemId == userId || t.CarteiraDestinoId == userId)
            .Where(t => (!inicio.HasValue || t.Data >= inicio) && (!fim.HasValue || t.Data <= fim))
            .ToList();
        return Ok(transferencias);
    }
}
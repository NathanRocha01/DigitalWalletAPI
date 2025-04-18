using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransferController : ControllerBase
{
    private readonly WalletDbContext _context;
    public TransferController(WalletDbContext context)
    {
        _context = context;
    }
    public IActionResult CreateTransfer([FromBody] TransferRequest request)
    {
        var originId = AuthHelper.GetUserIdFromToken(HttpContext);
        var originWallet = _context.Wallet.SingleOrDefault(c => c.UserId == originId);
        var destinationWallet = _context.Wallet.SingleOrDefault(c => c.UserId == request.DestinationId);

        if (originWallet.Amount >= request.Amount)
        {
            originWallet.Amount -= request.Amount;
            destinationWallet.Amount += request.Amount;

            _context.Transfers.Add(new Transfer { OriginWalletId = originId, DestinationWalletId = request.DestinationId, Amount = request.Amount, Date = DateTime.Now });
            _context.SaveChanges();

            return Ok();
        }

        return BadRequest("Insufficient balance");
    }

    [Authorize]
    [HttpGet("transfers")]
    public IActionResult ListTransfers(DateTime? begin, DateTime? end)
    {
        var userId = AuthHelper.GetUserIdFromToken(HttpContext);
        var transfer = _context.Transfers
            .Where(t => t.OriginWalletId == userId || t.DestinationWalletId == userId)
            .Where(t => (!begin.HasValue || t.Date >= begin) && (!end.HasValue || t.Date <= end))
            .ToList();
        return Ok(transfer);
    }
}
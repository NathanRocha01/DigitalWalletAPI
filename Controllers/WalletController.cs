using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WalletController : ControllerBase
{

    private readonly WalletDbContext _context;
    public WalletController(WalletDbContext context)
    {
        _context = context;
    }

    [HttpGet("wallet/balance")]
    public IActionResult CheckBalance()
    {
        var userId = AuthHelper.GetUserIdFromToken(HttpContext);
        var wallet = _context.Wallet.SingleOrDefault(c => c.UserId == userId);
        return Ok(new { Saldo = wallet.Amount });
    }

    [HttpPost("wallet/add")]
    public IActionResult AddBalance([FromBody] AddBalanceRequest request)
    {
        var userId = AuthHelper.GetUserIdFromToken(HttpContext);
        var wallet = _context.Wallet.SingleOrDefault(c => c.UserId == userId);
        wallet.Amount += request.Amount;
        _context.SaveChanges();
        return Ok();
    }

}


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WalletController : ControllerBase
{

    private readonly WalletService _wallet;
    public WalletController(WalletService wallet)
    {
        _wallet = wallet;
    }

    [HttpGet("wallet/balance")]
    public IActionResult CheckBalance()
    {
        try
        {
            var userId = AuthHelper.GetUserIdFromToken(HttpContext);
            var balance = _wallet.GetUserBalance(userId);
            return Ok(new { Saldo = balance });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("wallet/add")]
    public IActionResult AddBalance([FromBody] AddBalanceRequest request)
    {
        try
        {
            var userId = AuthHelper.GetUserIdFromToken(HttpContext);
            _wallet.AddBalance(userId, request.Amount);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

}


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransferController : ControllerBase
{
    private readonly TransferService _transfer;
    public TransferController(TransferService transfer)
    {
        _transfer = transfer;
    }
    [HttpPost]
    public IActionResult CreateTransfer([FromBody] TransferRequest request)
    {
        try
        {
            var originUserId = AuthHelper.GetUserIdFromToken(HttpContext);
            _transfer.ExecuteTransfer(originUserId, request);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("list")]
    public IActionResult ListTransfers(DateTime? begin, DateTime? end)
    {
        try
        {
            var userId = AuthHelper.GetUserIdFromToken(HttpContext);
            var transfers = _transfer.GetUserTransfers(userId, begin, end);
            return Ok(transfers);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
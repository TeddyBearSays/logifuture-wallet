using Microsoft.AspNetCore.Mvc;
using WalletSytem.BusinessLayer.Models;
using WalletSytem.BusinessLayer;

namespace WalletSytem.Controllers;



[ApiController]
[Route("api/[controller]")]
public class WalletController : ControllerBase
{
    private readonly IWalletService _walletService;
    private readonly ICorrelationIdProvider correlationIdProvider;

    public WalletController(IWalletService walletService, ICorrelationIdProvider correlationIdProvider)
    {
        _walletService = walletService;
        this.correlationIdProvider = correlationIdProvider;
       

    }

    [HttpPost]
    public async Task<ActionResult<Wallet>> CreateWallet(Guid userId, string currecny)
    {

        var wallet = await _walletService.CreateWalletAsync(userId, currecny);
        return Ok(wallet);
    }

    [HttpGet("{userId}/{walletId}")]
    public async Task<ActionResult<Wallet>> GetWallet(Guid walletId, Guid userId)
    {
       
        var wallet = await _walletService.GetWalletAsync(walletId, userId);
        if (wallet == null) return NotFound();

        return Ok(wallet);
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<Wallet>> GetAllWallets(Guid userId)
    {
        var wallet = await _walletService.GetAllWalletsAsync(userId);
        if (wallet == null) return NotFound();

        return Ok(wallet);
    }


    [HttpGet("{userId}/{walletId}/transactions")]
    public async Task<ActionResult<Wallet>> GetWalletTransactions(Guid walletId, Guid userId)
    {
        // TODO : no user wallet check
        var transactions = await _walletService.GetWalletTransactionsAsync(walletId);

        return Ok(transactions);
    }

    [HttpPost("{userId}/{walletId}/freeze")]
    public async Task<ActionResult<Wallet>> FreezeWalletAsync(Guid walletId, Guid userId)
    {
        await _walletService.FreezeWalletAsync(walletId, userId);

        return Ok();
    }


    [HttpPost("{walletId}/chage-funds")]
    public async Task<IActionResult> AddFunds(Guid walletId, [FromBody] Funds pounds)
    {
        await _walletService.ChangeFundsAsync(walletId, pounds);
        return NoContent();
    }


}
using Microsoft.AspNetCore.Mvc;
using WalletSytem.BusinessLayer.Models;
using WalletSytem.BusinessLayer;
using WalletSytem.Models;

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
    public async Task<ActionResult<Wallet>> CreateWallet(UserDTO user, string currecny)
    {

        var wallet = await _walletService.CreateWalletAsync(user, currecny);
        return Ok(wallet);
    }

    [HttpGet("{userId}/{walletId}")]
    public async Task<ActionResult<Wallet>> GetWallet(string userId, long walletId)
    {
       
        var wallet = await _walletService.GetWalletAsync(userId,walletId);
        if (wallet == null) return NotFound();

        return Ok(wallet);
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<Wallet>> GetAllWallets(string userId)
    {
        var wallet = await _walletService.GetAllWalletsAsync(userId);
        if (wallet == null) return NotFound();

        return Ok(wallet);
    }


    [HttpGet("{userId}/{walletId}/transactions")]
    public async Task<ActionResult<Wallet>> GetWalletTransactions(string userId, long walletId )
    {
        // TODO : no user wallet check
        var transactions = await _walletService.GetWalletTransactionsAsync(userId, walletId);

        return Ok(transactions);
    }

    [HttpPost("{userId}/{walletId}/freeze")]
    public async Task<ActionResult<Wallet>> FreezeWalletAsync(string userId, long walletId)
    {
        await _walletService.FreezeWalletAsync(userId, walletId);

        return Ok();
    }

    [HttpPost("{userId}/freeze")]
    public async Task<ActionResult<Wallet>> FreezeUserAsync(string userId)
    {
        await _walletService.FreezeUserAsync(userId);

        return Ok();
    }



    [HttpPost("{userId}/{walletId}/chage-funds")]
    public async Task<IActionResult> AddFunds(string userId, long walletId, [FromBody] Funds pounds)
    {
        await _walletService.ChangeFundsAsync(userId, walletId, pounds);
        return NoContent();
    }


}
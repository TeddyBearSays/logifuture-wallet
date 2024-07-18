using WalletSytem.BusinessLayer.Models;
using WalletSytem.DataAccess;

namespace WalletSytem.BusinessLayer;

public interface IWalletService
{
    Task<Wallet> CreateWalletAsync(Guid userId, string currecny);
    Task<Wallet> GetWalletAsync(Guid walletId, Guid userId);
    Task<List<Wallet>> GetAllWalletsAsync(Guid userId);
    Task ChangeFundsAsync(Guid walletId, Funds pounds);
    Task<List<Transaction>> GetWalletTransactionsAsync(Guid walletId);
    Task FreezeWalletAsync(Guid walletId, Guid userId);
}

public class WalletService : IWalletService
{
    private readonly IWalletRepository _walletRepository;
    private readonly ICorrelationIdProvider correlationIdProvider;

    public WalletService(IWalletRepository walletRepository, ICorrelationIdProvider correlationIdProvider)
    {
        _walletRepository = walletRepository;
        this.correlationIdProvider = correlationIdProvider;
    }

    public async Task<Wallet> CreateWalletAsync(Guid userId, string currecny)
    {
        var wallet = new Wallet { Id = Guid.NewGuid(), Balance = 0, Currency = currecny, UserId = userId };
        await _walletRepository.AddWalletAsync(wallet);
        return wallet;
    }

    public async Task<Wallet> GetWalletAsync(Guid walletId, Guid userId )
    {

        var wallet = await _walletRepository.GetWalletAsync(walletId);
        if (wallet?.UserId != userId)
            return null;
        return wallet;
    }

    public async Task<List<Wallet>> GetAllWalletsAsync(Guid userId)
    {
        return await _walletRepository.GetAllWalletsAsync(userId);
    }

   
    public async Task ChangeFundsAsync(Guid walletId, Funds opeartion)
    {
        var wallet = await _walletRepository.GetWalletAsync(walletId);
        if (wallet == null) throw new Exception("Wallet not found");
        if (wallet.Frozen) throw new ArgumentException("Wallet is Frozen");


        var transaction = CreateNew(wallet, opeartion);

        if (transaction.BalanceAfter < 0)
        {
            throw new ArgumentException("Insufficient funds");
        }

        if (await _walletRepository.HasTransactionAsync(transaction))
        {
            throw new ArgumentException("Transaction Already Applied");
        }

        await _walletRepository.AddTransactionAsync(transaction);


        wallet.Balance = transaction.BalanceAfter;

        await _walletRepository.UpdateWalletAsync(wallet);

        await _walletRepository.CommitTransactionAsync(transaction);
    }

    public async Task<List<Transaction>> GetWalletTransactionsAsync(Guid walletId) => 
        await _walletRepository.GetWalletTransactionsAsync(walletId);

    public async Task FreezeWalletAsync(Guid walletId, Guid userId)
    {
        var wallet = await _walletRepository.GetWalletAsync(walletId);

        if (wallet?.UserId != userId)
            throw new ArgumentException();
        await _walletRepository.FreezeWalletAsync(walletId);
    }

    private Transaction CreateNew(Wallet wallet, Funds opeartion)
    {
        var transaction = new Transaction
        {
            ProviderId = correlationIdProvider.CorrelationId,
            WalletId = wallet.Id,
            BalanceBefore = wallet.Balance,
            BalanceAfter = wallet.Balance,
            Timestamp = opeartion.Timestamp,
            Commited = false
        };

        switch (opeartion.Operation)
        {
            case Operation.Add:
                transaction.BalanceAfter += opeartion.Amount;

                break;
            case Operation.Remove:
                transaction.BalanceAfter -= opeartion.Amount;

                break;
        }
        return transaction;
    }


}

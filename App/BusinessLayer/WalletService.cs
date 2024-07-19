using WalletSytem.BusinessLayer.Models;
using WalletSytem.DataAccess;
using WalletSytem.Models;

namespace WalletSytem.BusinessLayer;

public interface IWalletService
{
    Task<Wallet> CreateWalletAsync(UserDTO user, string currecny);
    Task<Wallet> GetWalletAsync(string userId, long walletId);
    Task<List<Wallet>> GetAllWalletsAsync(string userId);
    Task ChangeFundsAsync(string userId, long walletId,  Funds pounds);
    Task<List<Transaction>> GetWalletTransactionsAsync(string userId, long walletId);
    Task FreezeWalletAsync(string userId, long walletId);
    Task FreezeUserAsync(string userId);
}

public class WalletService : IWalletService
{
    private readonly IWalletRepository _walletRepository;
    private readonly ICorrelationIdProvider correlationIdProvider;
    private readonly TransactionRepository transactionRepository;

    public WalletService(IWalletRepository walletRepository, ICorrelationIdProvider correlationIdProvider ,TransactionRepository transactionRepository)
    {
        _walletRepository = walletRepository;
        this.correlationIdProvider = correlationIdProvider;
        this.transactionRepository = transactionRepository;
    }

    public async Task<Wallet> CreateWalletAsync(UserDTO user, string currecny)
    {

        return await _walletRepository.AddWalletAsync(user, currecny);
    }

    public async Task<Wallet> GetWalletAsync(string userId, long walletId)
    {
        var wallet = await _walletRepository.GetWalletAsync(walletId);
        if (wallet?.UserId != userId) throw new Exception("Wallet/User pair is incorrect");
        if (wallet.User.Frozen) throw new Exception("Wallet/User is Frozen");
        return wallet;
    }

    public async Task<List<Wallet>> GetAllWalletsAsync(string userId)
    {
        return await _walletRepository.GetAllWalletsAsync(userId);
    }


    public async Task ChangeFundsAsync(string userId, long walletId, Funds opeartion)
    {
        var wallet = await _walletRepository.GetWalletAsync(walletId);
        if (wallet?.UserId != userId)  throw new Exception("Wallet/User pair is incorrect");
        if (wallet == null) throw new Exception("Wallet not found");
        if (wallet.Frozen) throw new ArgumentException("Wallet is Frozen");
        if (wallet.User.Frozen) throw new Exception("Wallet/User is Frozen");


        var transaction = CreateNew(wallet, opeartion);

        if (transaction.BalanceAfter < 0)
        {
            throw new ArgumentException("Insufficient funds");
        }

        if (await transactionRepository.HasTransactionAsync(transaction))
        {
            throw new ArgumentException("Transaction Already Applied");
        }

        await transactionRepository.AddTransactionAsync(transaction);


        wallet.Balance = transaction.BalanceAfter;

        await _walletRepository.UpdateWalletAsync(wallet);

        await transactionRepository.CommitTransactionAsync(transaction);
    }


    public async Task FreezeWalletAsync(string userId, long walletId)
    {
        var wallet = await _walletRepository.GetWalletAsync(walletId);

        if (wallet?.UserId != userId)
            if (wallet?.UserId != userId) throw new Exception("Wallet/User pair is incorrect");
        if (wallet.User.Frozen) throw new Exception("Wallet/User is Frozen");

        await _walletRepository.FreezeWalletAsync(walletId);
    }

    public async Task FreezeUserAsync(string userId)
    {
        await _walletRepository.FreezeUserAsync(userId);

        
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

    public async Task<List<Transaction>> GetWalletTransactionsAsync(string userId, long walletId)
    {
        return await transactionRepository.GetWalletTransactionsAsync(userId, walletId);
    }

}

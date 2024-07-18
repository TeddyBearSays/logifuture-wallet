using Microsoft.EntityFrameworkCore;
using WalletSytem.BusinessLayer.Models;

namespace WalletSytem.DataAccess;

public interface IWalletRepository
{
    Task<Wallet> GetWalletAsync(Guid walletId);
    Task AddWalletAsync(Wallet wallet);
    Task UpdateWalletAsync(Wallet wallet);
    Task AddTransactionAsync(Transaction transaction);
    Task CommitTransactionAsync(Transaction transaction);
    Task<List<Wallet>> GetAllWalletsAsync(Guid userId);
    Task<bool> HasTransactionAsync(Transaction transaction);
    Task<List<Transaction>> GetWalletTransactionsAsync(Guid walletId);
    Task FreezeWalletAsync(Guid walletId);
}

public class WalletRepository : IWalletRepository
{
    private readonly WalletDbContext _context;

    public WalletRepository(WalletDbContext context)
    {
        _context = context;
    }

    
    public async Task<List<Wallet>> GetAllWalletsAsync(Guid userId)
    {
        var user  =  await _context.Users.FindAsync(userId);
        if (user == null)
            return new List<Wallet>();
        return user.Wallets.ToList();
    }

    public async Task AddWalletAsync(Wallet wallet)
    {
        var user = await _context.Users.FindAsync(wallet.UserId);
        if(user == null)
        {
            user = new User { Id = Guid.NewGuid() };
            _context.Users.Add(user);
        }
        await _context.Wallets.AddAsync(wallet);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateWalletAsync(Wallet wallet)
    {
        _context.Wallets.Update(wallet);
        await _context.SaveChangesAsync();
    }

    public async Task AddTransactionAsync(Transaction transaction)
    {
        transaction.CalculateHash();
        await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync();
    }
    public async Task CommitTransactionAsync(Transaction transaction)
    {
        transaction.Commited = true;
        _context.Transactions.Update(transaction);
        await _context.SaveChangesAsync();
    }
    public async Task<bool> HasTransactionAsync(Transaction transaction)
    {
        var result = await _context.Transactions.FindAsync(transaction.Id);
        return result == null;
    }

    public async Task<List<Transaction>> GetWalletTransactionsAsync(Guid walletId)
    {
        var transactions =  _context.Transactions.Where(x => x.WalletId == walletId);
        return await transactions.ToListAsync();
    }

    public async Task<Wallet> GetWalletAsync(Guid walletId)
    {
        return await _context.Wallets.FindAsync(walletId);
        
    }

    public async Task FreezeWalletAsync(Guid walletId)
    {
        var wallet =  await _context.Wallets.FindAsync(walletId);
        if (wallet != null)
        {
            throw new InvalidOperationException();
        }
        wallet.Frozen = true;
        await _context.SaveChangesAsync();
    }
}
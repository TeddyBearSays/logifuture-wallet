using Microsoft.EntityFrameworkCore;
using WalletSytem.BusinessLayer.Models;

namespace WalletSytem.DataAccess;


public interface ITransactionRepository
{
    Task AddTransactionAsync(Transaction transaction);
    Task CommitTransactionAsync(Transaction transaction);
    Task<bool> HasTransactionAsync(Transaction transaction);
    Task<List<Transaction>> GetWalletTransactionsAsync(string userId, long walletId);
}

public class TransactionRepository : ITransactionRepository
{
    private readonly WalletDbContext _context;

    public TransactionRepository(WalletDbContext context)
    {
        _context = context;
    }

    public async Task AddTransactionAsync(Transaction transaction)
    {
        transaction.Id = transaction.CalculateHash();
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

    public async Task<List<Transaction>> GetWalletTransactionsAsync(string userId,long walletId)
    {
        var transactions = _context.Transactions.Where(x => x.WalletId == walletId);
        return await transactions.ToListAsync();
    }

 
}

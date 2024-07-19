using Microsoft.EntityFrameworkCore;
using WalletSytem.BusinessLayer.Models;
using WalletSytem.Models;

namespace WalletSytem.DataAccess;

public interface IWalletRepository
{
    Task<Wallet> GetWalletAsync(long walletId);
    Task<Wallet> AddWalletAsync(UserDTO user, string currecny);
    Task UpdateWalletAsync(Wallet wallet);
    Task<List<Wallet>> GetAllWalletsAsync(string userId);
    Task FreezeWalletAsync(long walletId);
    Task FreezeUserAsync(string userId);

}

public class WalletRepository : IWalletRepository
{
    private readonly WalletDbContext _context;

    public WalletRepository(WalletDbContext context)
    {
        _context = context;
    }

    
    public async Task<List<Wallet>> GetAllWalletsAsync(string userId)
    {
        var user  =  await _context.Users.FindAsync(userId);
        if (user == null)
            return new List<Wallet>();
        return user.Wallets.ToList();
    }

    public async Task<Wallet> AddWalletAsync(UserDTO userDTO, string currecny)
    {
        var id = userDTO.CalculateHash();
        var user = await _context.Users.FindAsync(id);
        
        if (user == null)
        {
            user = new User
            {
                Username = userDTO.Username,
                Email = userDTO.Email,
                DateCreated = userDTO.DateCreated,
                PasswordHash = userDTO.PasswordHash,
                Id = id,
                Wallets = new List<Wallet>()
            };
            
            _context.Users.Add(user);
        }
        if (user.Frozen)
        {
            throw new InvalidOperationException("User is frozen");
        }

        var wallet = new Wallet { Balance = 0, Currency = currecny, User = user };
        user.Wallets.Add(wallet);

        await _context.SaveChangesAsync();
        return wallet;
    }

    public async Task UpdateWalletAsync(Wallet wallet)
    {
        _context.Wallets.Update(wallet);
        await _context.SaveChangesAsync();
    }

    public async Task AddTransactionAsync(Transaction transaction)
    {
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

    public async Task<List<Transaction>> GetWalletTransactionsAsync(long walletId)
    {
        var transactions =  _context.Transactions.Where(x => x.WalletId == walletId);
        return await transactions.ToListAsync();
    }

    public async Task<Wallet> GetWalletAsync(long walletId)
    {

        return await _context.Wallets.Include(i => i.User)
            .FirstAsync(x=>x.Id == walletId);
        
    }

    public async Task FreezeWalletAsync(long walletId)
    {
        var wallet =  await _context.Wallets.FindAsync(walletId);
        if (wallet != null)
        {
            throw new InvalidOperationException();
        }
        wallet.Frozen = true;
        await _context.SaveChangesAsync();
    }
    public async Task FreezeUserAsync(string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            throw new InvalidOperationException();
        }
        user.Frozen = true;
        await _context.SaveChangesAsync();
    }
}
using Microsoft.EntityFrameworkCore;
using WalletSytem.BusinessLayer.Models;

namespace WalletSytem.DataAccess;

public class WalletDbContext : DbContext
{
    public WalletDbContext(DbContextOptions<WalletDbContext> options) : base(options) { }

    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Wallet>()
            .HasKey(w => w.Id);

        modelBuilder.Entity<Transaction>()
            .HasKey(t => t.Id);

        modelBuilder.Entity<Transaction>()
            .HasOne<Wallet>()
            .WithMany()
            .HasForeignKey(t => t.WalletId);

        modelBuilder.Entity<Wallet>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(t => t.UserId);

        modelBuilder.Entity<User>()
                .HasMany(u => u.Wallets)
                .WithOne()
                .HasForeignKey(w => w.UserId);
    }
}


    

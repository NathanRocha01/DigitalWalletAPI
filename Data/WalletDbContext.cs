using Microsoft.EntityFrameworkCore;

public interface IWalletDbContext
{
    DbSet<User> Users { get; set; }
    DbSet<Wallet> Wallet { get; set; }
    DbSet<Transfer> Transfers { get; set; }
    int SaveChanges();
}
public class WalletDbContext : DbContext, IWalletDbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Wallet> Wallet { get; set; }
    public DbSet<Transfer> Transfers { get; set; }

    public WalletDbContext(DbContextOptions<WalletDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(new User { Id = 1, Name = "Admin", Email = "admin@admin.com", Password = "hashed_password" });
    }
}
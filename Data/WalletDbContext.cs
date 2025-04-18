using Microsoft.EntityFrameworkCore;

public class WalletDbContext : DbContext
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
using Microsoft.EntityFrameworkCore;

public class CarteiraDbContext : DbContext
{
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Carteira> Carteiras { get; set; }
    public DbSet<Transferencia> Transferencias { get; set; }

    public CarteiraDbContext(DbContextOptions<CarteiraDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>().HasData(new Usuario { Id = 1, Nome = "Admin", Email = "admin@admin.com", Senha = "hashed_password" });
    }
}
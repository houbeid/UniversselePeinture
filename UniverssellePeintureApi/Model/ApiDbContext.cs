using Microsoft.EntityFrameworkCore;

namespace UniverssellePeintureApi.Model
{
    public class ApiDbContext : DbContext
    {

        public ApiDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Commerce> Commerces { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Produit> Produits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Commerce>()
                .HasMany(c => c.Clients)
                .WithOne(c => c.Commerce)
                .HasForeignKey(c => c.CommercantId);

            modelBuilder.Entity<Client>()
                .HasMany(c => c.Stocks)
                .WithOne(s => s.Client)
                .HasForeignKey(s => s.ClientId);

            modelBuilder.Entity<Produit>()
                .HasMany(p => p.Stocks)
                .WithOne(s => s.Produit)
                .HasForeignKey(s => s.ProduitId);

            modelBuilder.Entity<Client>()
                .HasIndex(c => c.Code)
                .IsUnique();
        }
    }
}

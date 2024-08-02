using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace UniverssellePeintureApi.Model
{
    public class ApiDbContext : IdentityDbContext<User>
    {

        public ApiDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Commerce> Commerces { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Produit> Produits { get; set; }
        public DbSet<StockProduit> StockProduits { get; set; }

        public DbSet<PortFeuilleClient> portFeuilleClients { get; set; }

        public DbSet<Historique> Historiques { get; set; }

        public DbSet<Facture> Factures { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Commerce>()
                .HasMany(c => c.Clients)
                .WithOne(c => c.Commerce)
                .HasForeignKey(c => c.CommercantId);
            modelBuilder.Entity<Commerce>()
                .HasMany(c => c.PortFeuilleClients)
                .WithOne(c => c.Commerce)
                .HasForeignKey(c => c.CommercantId);

            modelBuilder.Entity<Client>()
                .HasMany(c => c.Stocks)
                .WithOne(s => s.Client)
                .HasForeignKey(s => s.ClientId);

            modelBuilder.Entity<Client>()
                .HasMany(c => c.Historiques)
                .WithOne(s => s.Client)
                .HasForeignKey(s => s.ClientId);

            modelBuilder.Entity<Client>()
                .HasMany(c => c.Factures)
                .WithOne(s => s.Client)
                .HasForeignKey(s => s.ClientId);

            modelBuilder.Entity<StockProduit>()
           .HasKey(sp => new { sp.StockId, sp.ProduitId });

            modelBuilder.Entity<StockProduit>()
                .HasOne(sp => sp.Stock)
                .WithMany(s => s.StockProduits)
                .HasForeignKey(sp => sp.StockId);

            modelBuilder.Entity<StockProduit>()
                .HasOne(sp => sp.Produit)
                .WithMany(p => p.StockProduits)
                .HasForeignKey(sp => sp.ProduitId);

            modelBuilder.Entity<Client>()
                .HasIndex(c => c.Code)
                .IsUnique();
            modelBuilder.Entity<Commerce>()
                .HasMany(c => c.PortFeuilleClients)
                .WithOne(c => c.Commerce)
                .HasForeignKey(c => c.CommercantId);

            modelBuilder.Entity<PortFeuilleClient>()
                .HasIndex(c => c.Code)
                .IsUnique();
            
        }
    }
}

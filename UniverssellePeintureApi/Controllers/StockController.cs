using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniverssellePeintureApi.DTO;
using UniverssellePeintureApi.Model;

namespace UniverssellePeintureApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public StockController(ApiDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task AddStock([FromBody]  AddStockdto addStockDto)
        {
            // Trouver le client par code
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Code == addStockDto.CodeClient);
            if (client == null)
            {
                throw new Exception("Client not found");
            }

            // Créer un nouveau stock
            var newStock = new Stock
            {
                Quantity = addStockDto.StockProduitdto.Sum(sp => sp.Quantite),
                PrixDeVenteTotal = 0, // Vous pouvez calculer le prix total si nécessaire
                ClientId = client.Id
            };

            _context.Stocks.Add(newStock);
            await _context.SaveChangesAsync();

            // Ajouter les produits au stock et mettre à jour les pourcentages de vente
            foreach (var stockProduitDto in addStockDto.StockProduitdto)
            {
                var produit = await _context.Produits.FirstOrDefaultAsync(p => p.Name == stockProduitDto.NameProduit);
                if (produit != null)
                {
                    produit.StockActuel += stockProduitDto.Quantite;

          

                    var stockProduit = new StockProduit
                    {
                        StockId = newStock.Id,
                        ProduitId = produit.Id
                    };

                    _context.StockProduits.Add(stockProduit);
                }
            }

            await _context.SaveChangesAsync();
        }

        [HttpPost]
        public async Task UpdateStockAsync(UpdateStockdto updateStockDto)
        {
            // Trouver le client par code
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Code == updateStockDto.CodeClient);
            if (client == null)
            {
                throw new Exception("Client not found");
            }

            // Trouver le stock associé à ce client
            var stock = await _context.Stocks.Include(s => s.StockProduits).FirstOrDefaultAsync(s => s.ClientId == client.Id);
            if (stock == null)
            {
                throw new Exception("Stock not found for this client");
            }

            // Mettre à jour les informations du stock
            stock.PrixDeVenteTotal = updateStockDto.PrixDeVenteTotal;
            stock.Description = updateStockDto.Description;
            stock.RecetteJourne = updateStockDto.RecetteJourne;
            stock.Quantity = updateStockDto.StockProduitDto.Sum(sp => sp.Quantite);

            // Supprimer les anciennes associations de produits
            _context.StockProduits.RemoveRange(stock.StockProduits);

            // Ajouter les nouvelles associations de produits et mettre à jour les informations de stock
            foreach (var stockProduitDto in updateStockDto.StockProduitDto)
            {
                var produit = await _context.Produits.FirstOrDefaultAsync(p => p.Name == stockProduitDto.NameProduit);
                if (produit != null)
                {
                    produit.StockActuel += stockProduitDto.Quantite;

                    // Calculer le pourcentage de vente
                    produit.PourcentageVente = (decimal)produit.StockActuel / await _context.Produits.SumAsync(p => p.StockActuel) * 100;

                    var stockProduit = new StockProduit
                    {
                        StockId = stock.Id,
                        ProduitId = produit.Id
                    };

                    _context.StockProduits.Add(stockProduit);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}

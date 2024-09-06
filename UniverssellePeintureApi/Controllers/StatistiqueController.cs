using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using UniverssellePeintureApi.DTO;
using UniverssellePeintureApi.DTO.Response;
using UniverssellePeintureApi.Model;

namespace UniverssellePeintureApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatistiqueController : ControllerBase
    {
        private readonly ApiDbContext _context;

        [HttpGet]
        public async Task<List<StatistiqueResponse>> GetStatistique()
        {
            var produits = await _context.Produits.ToListAsync();
            if (produits == null)
            {
                throw new ArgumentException("Invalid statistique", nameof(produits));
            }
            var statistiqueList = new List<StatistiqueResponse>();
            int stock_fabrique_total = 0;
            int stock_actuel_total = 0;
            foreach (var Produit in produits)
            {
                statistiqueList.Add(new StatistiqueResponse()
                {
                    produit = Produit.Name,
                    stock_fabrique = Produit.stock,
                    stock_actuel = Produit.StockActuel,
                    pourcentage_produit = Produit.PourcentageVente
                });
                stock_fabrique_total += Produit.stock;
                stock_actuel_total += Produit.StockActuel;
            }
            statistiqueList.Add(new StatistiqueResponse()
            {
                produit = "Total",
                stock_fabrique = stock_fabrique_total,
                stock_actuel = stock_actuel_total,
                pourcentage_vent = ((stock_fabrique_total - stock_actuel_total) / stock_fabrique_total) * 100
            });
            return statistiqueList;
        }
    }
}

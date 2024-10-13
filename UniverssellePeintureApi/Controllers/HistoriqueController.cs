using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniverssellePeintureApi.DTO;
using UniverssellePeintureApi.Model;
using UniverssellePeintureApi.DTO.Response;
using Microsoft.AspNetCore.Authorization;

namespace UniverssellePeintureApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoriqueController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public HistoriqueController(ApiDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public async Task<List<HistoriqueResponse>> GetHistoriqueClient(string codeClient)
        {
            var client = await _context.Clients.Where(c => c.Code == codeClient).
                Include(a => a.Historiques)
                .FirstOrDefaultAsync();
            if (client == null)
            {
                throw new ArgumentException("Invalid client", nameof(Historique));
            }
            decimal montant_total = 0;
            var HistoriqueClient = new List<HistoriqueResponse>();
            foreach (var clienthistorique in client.Historiques) 
            {
                HistoriqueClient.Add(
                    new HistoriqueResponse
                    {
                        Produit = clienthistorique.NameProduit,
                        Quantite = clienthistorique.Quantite,
                        Date = clienthistorique.Delivery_date,
                        Montant = clienthistorique.Montant,
                        Commercial = clienthistorique.distributeur
                    }
                    );
                montant_total += clienthistorique.Montant;
            }
            HistoriqueClient.Add(
                new HistoriqueResponse
                {
                    Produit = "Chiffre D'affaire",
                    Montant = montant_total
                });
            return HistoriqueClient;
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniverssellePeintureApi.DTO;
using UniverssellePeintureApi.Model;
using UniverssellePeintureApi.DTO.Response;

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
            var HistoriqueClient = new List<HistoriqueResponse>();
            foreach (var clienthistorique in client.Historiques) 
            {
                HistoriqueClient.Add(
                    new HistoriqueResponse
                    {
                        NameProduit = clienthistorique.NameProduit,
                        Quantity = clienthistorique.Quantite,
                        delivery_date = clienthistorique.Delivery_date,
                        montant = clienthistorique.Montant,
                        distributeur = clienthistorique.distributeur
                    }
                    );
            }
            return HistoriqueClient;
        }
    }
}

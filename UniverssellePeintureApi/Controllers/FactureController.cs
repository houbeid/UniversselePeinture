using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniverssellePeintureApi.DTO;
using UniverssellePeintureApi.DTO.Response;
using UniverssellePeintureApi.Model;

namespace UniverssellePeintureApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FactureController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public FactureController(ApiDbContext context)
        {
            _context = context;
        }

        [HttpPost("Add")]
        public async Task<IActionResult> CreateFacture([FromBody] AddFactureDto factureDto)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Code == factureDto.CodeClient);
            if (client == null)
            {
                throw new Exception("Client not found");
            }
            var facture = new Facture
            {
                date = factureDto.date,
                Adress = client.Zone,
                Code = factureDto.CodeClient,
                facture = factureDto.Facture,
                client = client.Respnsible_Name,
                Montant = factureDto.Montant,
                distributeur = factureDto.distribiteur,
                ClientId = client.Id
            };
            _context.Factures.Add(facture);
            await _context.SaveChangesAsync();
            return Ok("facture created successfully.");
        }

        [HttpGet]
        public async Task<List<FactureResponse>>GetFacture()
        {
            try
            {
                var Facture = await _context.Factures
                    .Where(c => c.date == DateTime.Today)
                    .Select(c => new FactureResponse
                    {
                        date = c.date,
                        Adress = c.Adress,
                        Code = c.Code,
                        client = c.client,
                        facture = c.facture,
                        Montant = c.Montant,
                        distributeur = c.distributeur
                    })
     .ToListAsync();

                if (Facture == null || !Facture.Any())
                {
                    throw new ArgumentException("Invalid facture", nameof(Commerce));
                }

                return Facture;
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here if needed
                throw new ArgumentException("Invalid Facture", nameof(Commerce));
            }
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniverssellePeintureApi.DTO;
using UniverssellePeintureApi.Model;

namespace UniverssellePeintureApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public ClientsController(ApiDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateClient([FromBody] AddClientDto clientDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var commerce = await _context.Commerces.FindAsync(clientDto.CommercantId);
            if (commerce == null)
            {
                return NotFound("Commerce not found.");
            }

            var client = new Client
            {
                Code = clientDto.Code,
                Name_Society = clientDto.Name_Society,
                Phone_Number = clientDto.Phone_Number,
                Respnsible_Name = clientDto.Respnsible_Name,
                Gérant = clientDto.Gérant,
                Solvabilité = clientDto.Solvabilité,
                CoordonnéesGPS = clientDto.CoordonnéesGPS,
                Zone = clientDto.Zone,
                Recommandation = clientDto.Recommandation,
                Visit_Date = clientDto.Visit_Date,
                Delivery_Date = clientDto.Delivery_Date,
                Description = clientDto.Description,
                CommercantId = clientDto.CommercantId,
                Commerce = commerce,
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClientById), new { id = client.Id }, client);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClientById(int id)
        {
            var client = await _context.Clients.Include(c => c.Commerce).FirstOrDefaultAsync(c => c.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return Ok(client);
        }
    }

}

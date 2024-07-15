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

        [HttpPost("{update}")]
        public async Task<bool> UpdateClientAsync(ClientUpdateRequest updateRequest)
        {
            var client = await _context.Clients
            .Where(c => c.Code == updateRequest.Code)
                .FirstOrDefaultAsync();

            if (client == null)
            {
                return false; // Client not found
            }
            if (updateRequest.Name_Society != null && updateRequest.Name_Society != client.Name_Society)
            {
                client.Name_Society = updateRequest.Name_Society;
            }
            if (updateRequest.Phone_Number != null && updateRequest.Phone_Number != client.Phone_Number)
                client.Phone_Number = updateRequest.Phone_Number;
            if (updateRequest.Respnsible_Name != null && updateRequest.Respnsible_Name != client.Respnsible_Name)
                client.Respnsible_Name = updateRequest.Respnsible_Name;
            if (updateRequest.Gérant != null && updateRequest.Gérant != client.Gérant)
                client.Gérant = updateRequest.Gérant;
            if (updateRequest.Solvabilité != null && updateRequest.Solvabilité != client.Solvabilité)
                client.Solvabilité = updateRequest.Solvabilité;
            if (updateRequest.CoordonnéesGPS != null && updateRequest.CoordonnéesGPS != client.CoordonnéesGPS)
                client.CoordonnéesGPS = updateRequest.CoordonnéesGPS;
            if (updateRequest.Zone != null && updateRequest.Zone != client.Zone)
                client.Zone = updateRequest.Zone;
            if (updateRequest.Recommandation != null && updateRequest.Recommandation != client.Recommandation)
                client.Recommandation = updateRequest.Recommandation;
            if (updateRequest.Visit_Date != null && updateRequest.Visit_Date != client.Visit_Date)
                client.Visit_Date = updateRequest.Visit_Date;
            if (updateRequest.Delivery_Date != null && updateRequest.Delivery_Date != client.Delivery_Date)
                client.Delivery_Date = updateRequest.Delivery_Date;
            if (updateRequest.Description != null && updateRequest.Description != client.Description)
                client.Description = updateRequest.Description;

            _context.Clients.Update(client);
            await _context.SaveChangesAsync();

            return true;
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

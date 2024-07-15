using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniverssellePeintureApi.Model;
using UniverssellePeintureApi.DTO;
using Microsoft.EntityFrameworkCore;
using UniverssellePeintureApi.DTO.Response;
using System.Linq;

namespace UniverssellePeintureApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommercesController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public CommercesController(ApiDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCommerce([FromBody] AddCommerceDto commerceDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var commerce = new Commerce
            {
                Nom = commerceDto.Nom,
                Telephone = commerceDto.Telephone
            };

            _context.Commerces.Add(commerce);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCommerceById), new { id = commerce.Id }, commerce);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCommerceById(int id)
        {
            var commerce = await _context.Commerces.FirstOrDefaultAsync(c => c.Id == id);
            if (commerce == null)
            {
                return NotFound();
            }

            return Ok(commerce);
        }

        


        [HttpGet]
        public async Task<List<ComerceResponse>> GetAllCommerces()
        {
            try
            {
                var commerces = await _context.Commerces
                .Include(c => c.Clients)
                .Select(c => new ComerceResponse
                {
                    Id = c.Id,
                    Nom = c.Nom,
                    Telephone = c.Telephone,
                    Clients = c.Clients.Select(client => new ClientResponse
                    {
                         Id = client.Id,
                         Code = client.Code,
                         Name_Society = client.Name_Society,
                         Phone_Number = client.Phone_Number,
                         Respnsible_Name = client.Respnsible_Name,
                         Gérant = client.Gérant,
                         Solvabilité = client.Solvabilité,
                         CoordonnéesGPS = client.CoordonnéesGPS,
                         Zone = client.Zone,
                         Recommandation = client.Recommandation,
                         Visit_Date = client.Visit_Date,
                         Delivery_Date = client.Delivery_Date,
                         Description = client.Description
                    }).ToList()
                })
                .ToListAsync();

                if (commerces == null || !commerces.Any())
                {
                    throw new ArgumentException("Invalid Comerce", nameof(Commerce));
                }

                return commerces;
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here if needed
                throw new ArgumentException("Invalid Comerce", nameof(Commerce));
            }
        }
    }
}

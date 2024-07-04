using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniverssellePeintureApi.Model;
using UniverssellePeintureApi.DTO;
using Microsoft.EntityFrameworkCore;

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
    }

}

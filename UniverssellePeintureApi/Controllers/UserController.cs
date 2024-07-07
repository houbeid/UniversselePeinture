using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UniverssellePeintureApi.Model;
using UniverssellePeintureApi.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace UniverssellePeintureApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public UserController(UserManager<User> userManager, IConfiguration _configuration) {

            _userManager = userManager;
        }

        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration; 

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterNewUser(AddUserDto model)
        {
            if (ModelState.IsValid)
            {
                User user = new()
                {
                    UserName = model.userName,
                    PhoneNumber = model.phone
                };
                var result = await _userManager.CreateAsync(user, model.password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User"); // Assign default role
                    if (model.IsAdmin)
                    {
                        await _userManager.AddToRoleAsync(user, "Admin");
                    }
                    return Ok(new { Result = "User registered successfully" });
                }
                else
                {
                    foreach(var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }

            }
            return BadRequest(ModelState);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (ModelState.IsValid)
            {
                User? user = await _userManager.FindByNameAsync(model.userName);
                if (user != null)
                { 
                    var token = GenerateJwtToken(user);
                    return Ok(new { Token = token });
                }
                return Unauthorized();
            }
            return BadRequest(ModelState);
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

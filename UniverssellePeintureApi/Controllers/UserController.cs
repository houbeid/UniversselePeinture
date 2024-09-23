﻿using Microsoft.AspNetCore.Http;
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
        public UserController(UserManager<User> userManager, IConfiguration configuration, ApiDbContext context) {

            _userManager = userManager;
            _configuration = configuration;
            _context = context;
        }

        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApiDbContext _context;

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
                    var comercial = await _context.Commerces
                    .Where(c => c.Telephone == model.phone)
                        .FirstOrDefaultAsync();
                    if (comercial != null)
                    {
                        ModelState.AddModelError("Commercial: ", "Commercial exist deja!");
                    }
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }

                    var commerce = new Commerce
                    {
                        Nom = model.userName,
                        Telephone = model.phone
                    };

                    _context.Commerces.Add(commerce);
                    await _context.SaveChangesAsync();
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
                    if (await _userManager.CheckPasswordAsync(user, model.password)) 
                    {
                        var claims = new List<Claim>();
                        //new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                        claims.Add(new Claim(ClaimTypes.Name, user.UserName));
                        claims.Add(new Claim(ClaimTypes.MobilePhone, user.PhoneNumber));
                        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
                        //new Claim(JwtRegisteredClaimNames.Email, user.Email),
                        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                        var roles = await _userManager.GetRolesAsync(user);
                        foreach (var role in roles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
                        }

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        var token = new JwtSecurityToken(
                            issuer: _configuration["JWT:Issuer"],
                            audience: _configuration["JWT:Audience"],
                            claims: claims,
                            expires: DateTime.Now.AddMinutes(30),
                            signingCredentials: creds);
                        //var _token = new
                        //{
                        //    token = new JwtSecurityTokenHandler().WriteToken(token),
                        //    expiratin = token.ValidTo,
                        //};
                        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                        var clients = await _context.Clients
                            .Where(c => EF.Functions.DateDiffDay(c.Visit_Date, DateTime.Now) >= 7)
                            .ToListAsync();
                        var result = new
                        {
                            Token = tokenString,
                            Expiration = token.ValidTo,
                            Clients = clients
                        };
                        return Ok(result);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            return BadRequest(ModelState);

        }

        //private string GenerateJwtToken(User user)
        //{
        //    var claims = new List<Claim>();
        //    //new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        //     claims.Add(new Claim(ClaimTypes.Name, user.UserName));
        //    claims.Add(new Claim(ClaimTypes.MobilePhone, user.PhoneNumber));
        //    claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
        //    //new Claim(JwtRegisteredClaimNames.Email, user.Email),
        //    claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()))
        //        var roles await _userManager.GetRolesAsync(user);
        //    foreach(var role in roles)
        //    {
        //        claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
        //    }

        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //    var token = new JwtSecurityToken(
        //        issuer: _configuration["Jwt:Issuer"],
        //        audience: _configuration["Jwt:Audience"],
        //        claims: claims,
        //        expires: DateTime.Now.AddMinutes(30),
        //        signingCredentials: creds);


        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}
    }
}

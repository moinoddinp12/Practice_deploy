using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Online_Car_Marketplace.Data;
using Online_Car_Marketplace.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;


namespace Online_Car_Marketplace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        

        private readonly IConfiguration _config;

        public AuthApiController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        [Authorize(Roles ="Admin")]
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("You are authorized!");
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] User Model)
        {
            var allUsers = _context.Users.ToList();
            var user = _context.Users.FirstOrDefault(u => u.Username == Model.Username && u.Password == Model.Password);

            if(user == null )
            {
                return Unauthorized("invalid user or pass");

            }
            var token = GenerateToken(user.Username, user.Role);

            return Ok(new
            {
                message = "Login successful",
                token = token,
            });
        }

        private string GenerateToken(string username, string role)
        {
            var claims = new[]
            {
        new Claim(ClaimTypes.Name, username),
        new Claim(ClaimTypes.Role, role)
    };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"])
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }

}

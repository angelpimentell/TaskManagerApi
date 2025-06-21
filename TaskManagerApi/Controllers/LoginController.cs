using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagerApi.Data;
using TaskManagerApi.Services;

namespace LocalRNC.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [AllowAnonymous]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _context;
        private JwtTokenService _jwtTokenService;

        public LoginController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _jwtTokenService = new JwtTokenService(config);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            string email = request.Email;
            string password = request.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return BadRequest(new { Message = "Email and password are required." });
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);


            if (user != null && email == user.Email && password == user.Password)
            {
                
                var tokenString = _jwtTokenService.GenerateJWT();
                return Ok(new { Token = tokenString });
            }

            return Unauthorized();
        }
    }
}
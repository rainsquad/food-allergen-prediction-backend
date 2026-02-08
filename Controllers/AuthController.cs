using food_allergen_prediction_backend.Data;
using food_allergen_prediction_backend.DTOs;
using food_allergen_prediction_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace food_allergen_prediction_backend.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController(AppDbContext db) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (await db.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("Email already exists");

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            return Ok("Registered successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null ||
                !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials");

            return Ok(new
            {
                userId = user.Id,
                name = user.Name,
                email = user.Email
            });
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await db.Users.ToListAsync();

            return Ok(users);
        }
    }
}

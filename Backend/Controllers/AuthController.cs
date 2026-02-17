using Backend.Data;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher _hasher;
        private readonly JwtTokenService _jwt;

        public AuthController(
            ApplicationDbContext context,
            PasswordHasher hasher,
            JwtTokenService jwt)
        {
            _context = context;
            _hasher = hasher;
            _jwt = jwt;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return BadRequest("Email already registered");

            var user = new User
            {
                Email = request.Email,
                Name = request.Name,
                PasswordHash = _hasher.Hash(request.Password),
                Role = "user"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !_hasher.Verify(request.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials");

            var token = _jwt.GenerateToken(user);

            return Ok(new
            {
                token,
                user = new { user.Id, user.Email, user.Name, user.Role }
            });
        }

        [HttpGet("me")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Me()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;

            var user = await _context.Users.FindAsync(Guid.Parse(userId));

            return Ok(new { user!.Id, user.Email, user.Name, user.Role });
        }
    }

    public record RegisterRequest(string Email, string Password, string Name);
    public record LoginRequest(string Email, string Password);
}

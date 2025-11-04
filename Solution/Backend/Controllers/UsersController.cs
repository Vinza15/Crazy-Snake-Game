using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public UsersController(ApiDbContext context)
        {
            _context = context;
            _context.Database.EnsureCreated();
        }

        // POST: api/users/login
        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(User loginAttempt)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.PlayerName == loginAttempt.PlayerName &&
                u.Password == loginAttempt.Password);

            if (user == null)
            {
                return NotFound("Username atau password salah.");
            }
            return Ok(user);
        }

        // POST: api/users/register
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(User newUser)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.PlayerName == newUser.PlayerName);
            if (existingUser != null)
            {
                return Conflict("Username ini sudah dipakai.");
            }

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Login), new { id = newUser.Id }, newUser);
        }
    }
}

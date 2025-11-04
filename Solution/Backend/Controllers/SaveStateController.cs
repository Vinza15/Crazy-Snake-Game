using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaveStateController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public SaveStateController(ApiDbContext context)
        {
            _context = context;
            _context.Database.EnsureCreated();
        }

        // GET: api/savestate/{userId}
        [HttpGet("{userId}")]
        public async Task<ActionResult<SaveState>> GetSaveState(int userId)
        {
            var saveState = await _context.SaveStates.FirstOrDefaultAsync(s => s.UserId == userId);
            if (saveState == null)
            {
                return NotFound();
            }
            return Ok(saveState);
        }

        // POST: api/savestate
        [HttpPost]
        public async Task<ActionResult> PostSaveState(SaveState saveState)
        {
            var existingState = await _context.SaveStates.FirstOrDefaultAsync(s => s.UserId == saveState.UserId);

            if (existingState != null)
            {
                // Update save yang ada
                existingState.Score = saveState.Score;
                existingState.Level = saveState.Level;
                existingState.Direction = saveState.Direction;
                existingState.SnakeBodyJson = saveState.SnakeBodyJson;
                existingState.FoodPositionJson = saveState.FoodPositionJson;
            }
            else
            {
                // Buat save baru
                _context.SaveStates.Add(saveState);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        // DELETE: api/savestate/{userId}
        [HttpDelete("{userId}")]
        public async Task<ActionResult> DeleteSaveState(int userId)
        {
            var saveState = await _context.SaveStates.FirstOrDefaultAsync(s => s.UserId == userId);
            if (saveState != null)
            {
                _context.SaveStates.Remove(saveState);
                await _context.SaveChangesAsync();
            }
            return Ok();
        }
    }
}

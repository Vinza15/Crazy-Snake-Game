using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScoresController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public ScoresController(ApiDbContext context)
        {
            _context = context;
            _context.Database.EnsureCreated();
        }

        // GET: api/scores
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HighScore>>> GetHighScores()
        {
            return await _context.HighScores
                             .OrderByDescending(s => s.Score)
                             .ThenByDescending(s => s.Level)
                             .Take(10)
                             .ToListAsync();
        }

        // POST: api/scores
        [HttpPost]
        public async Task<ActionResult<HighScore>> PostHighScore(HighScore highScore)
        {
            _context.HighScores.Add(highScore);
            await _context.SaveChangesAsync();

            return Ok(highScore);
        }
    }
}

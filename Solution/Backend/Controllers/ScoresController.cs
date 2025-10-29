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
        }

        // GET: api/scores
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HighScore>>> GetHighScores()
        {
            // Pastikan database dibuat
            await _context.Database.EnsureCreatedAsync();

            // Ambil 10 skor tertinggi
            var scores = await _context.HighScores
                                     .OrderByDescending(s => s.Score)
                                     .Take(10)
                                     .ToListAsync();
            return Ok(scores);
        }

        // POST: api/scores
        [HttpPost]
        public async Task<ActionResult<HighScore>> PostHighScore(HighScore highScore)
        {
            // Pastikan database dibuat
            await _context.Database.EnsureCreatedAsync();

            _context.HighScores.Add(highScore);
            await _context.SaveChangesAsync();

            // Mengembalikan 'Created' bersama dengan data yang baru dibuat
            return CreatedAtAction(nameof(GetHighScores), new { id = highScore.Id }, highScore);
        }
    }
}

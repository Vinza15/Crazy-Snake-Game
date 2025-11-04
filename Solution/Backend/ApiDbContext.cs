using Microsoft.EntityFrameworkCore;

namespace Backend
{

    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) { }

        public DbSet<HighScore> HighScores { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<SaveState> SaveStates { get; set; }
    }
}

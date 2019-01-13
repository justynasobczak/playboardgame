using Microsoft.EntityFrameworkCore;

namespace PlayBoardGame.Models
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) { }

        public DbSet<Game> Games { get; set; }
    }
}

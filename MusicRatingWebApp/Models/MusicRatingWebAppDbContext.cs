using Microsoft.EntityFrameworkCore;

namespace MusicRatingWebApp.Models
{
    public class MusicRatingWebAppDbContext : DbContext
    {
        public MusicRatingWebAppDbContext(DbContextOptions<MusicRatingWebAppDbContext> options) : base(options)
        {
        }

        public DbSet<Artist> Artists { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<User> Users { get; set; }
    }
}

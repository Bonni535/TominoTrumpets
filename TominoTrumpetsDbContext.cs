using Microsoft.EntityFrameworkCore;
using TominoTrumpets.Models;

public class TominoTrumpetsDbContext : DbContext
{

    public DbSet<Artist>? Artists { get; set; }
    public DbSet<Genre>? Genres { get; set; }
    public DbSet<Song>? Songs { get; set; }
    public DbSet<Song_Genre>? Song_Genres { get; set; }

    public TominoTrumpetsDbContext(DbContextOptions<TominoTrumpetsDbContext> context) : base(context)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // seed data with Artist types
        modelBuilder.Entity<Artist>().HasData(new Artist[]
            {
            new Artist {
                Id = 1,
                Name = "Bruce Springsteen",
                Age = 74,
                Bio = "Bruce Frederick Joseph Springsteen (born September 23, 1949) is an American rock singer, songwriter and guitarist. Nicknamed \"the Boss\",[2] he has released 21 studio albums during a career spanning six decades, most of which feature his backing band, the E Street Band"
            },

            new Artist
            {
                Id = 2,
                Name = "Macklemore",
                Age= 40,
                Bio ="Benjamin Hammond Haggerty, better known by his stage name Macklemore, is an American rapper. A native of Seattle, Washington, he started his career in 2000 as an independent artist and released three works: Open Your Eyes (2000), The Language of My World (2005) and The Unplanned Mixtape (2009). He rose to international success when he collaborated with producer Ryan Lewis as the duo Macklemore & Ryan Lewis (2009–2016)."
            },
            });
        modelBuilder.Entity<Genre>().HasData(new Genre[]
        {
            new Genre
            {
                Id= 1,
                Description = "American Rock"
            },
            new Genre
            {
                Id= 2,
                Description = "Hip hop"
            }
        });

        modelBuilder.Entity<Song>().HasData(new Song[]
        {
            new Song
            {
                Id = 1,
                Title = "Born in the U.S.A.",
                ArtistId = 1,
                Album = "Born in the U.S.A.",
                Length = 437
            },

            new Song
            {
                Id = 2,
                Title = "Good Old Days",
                ArtistId = 2,
                Album = "Gemini",
                Length = 401
            }
        });

        modelBuilder.Entity<Song_Genre>().HasData(new Song_Genre[]
        {
            new Song_Genre
            {
                Id = 1,
                SongId = 1,
                GenreId = 1,
            },

            new Song_Genre
            {
                Id = 2,
                SongId= 2,
                GenreId= 2,
            }
        });
    }
}
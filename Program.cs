using TominoTrumpets.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// allows passing datetimes without time zone data 
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// allows our api endpoints to access the database through Entity Framework Core
builder.Services.AddNpgsql<TominoTrumpetsDbContext>(builder.Configuration["TominoTrumpetsDbConnectionString"]);

// Set the JSON serializer options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


// Song Api Calls

// Create a Song (Post)
app.MapPost("/api/songs", (TominoTrumpetsDbContext db, Song newSong) =>
{
    db.Songs.Add(newSong);
    db.SaveChanges();
    return Results.Created($"/api/songs/{newSong.Id}", newSong);
});

// Delete a Song (Delete)
app.MapDelete("/api/songs/{songId}", (TominoTrumpetsDbContext db, int id) =>
{
    var song = db.Songs.SingleOrDefault(s => s.Id == id);

    if(song == null)
    {
        return Results.NotFound();
    }

    db.Songs.Remove(song);
    db.SaveChanges();
    return Results.NoContent();
});

// Update a Song (Put)
app.MapPut("/api/songs/{songId}", (TominoTrumpetsDbContext db, Song song, int songId) =>
{
    Song updateSong = db.Songs.SingleOrDefault(s => s.Id == songId);
    if (updateSong == null)
    {
        return Results.NotFound();
    }
    updateSong.Title = song.Title;
    updateSong.ArtistId = song.ArtistId;
    updateSong.Album = song.Album;
    updateSong.Length = song.Length;

    db.SaveChanges();
    return Results.Created($"/api/songs/{song.Id}", song);
});

// View a List of all the Songs
app.MapGet("/api/songs", (TominoTrumpetsDbContext db) =>
{
    return db.Songs.ToList();
});

// View all the Songs by Id and Include Artist and Genre
app.MapGet("/api/songs/{songId}", (TominoTrumpetsDbContext db, int songId) =>
{
    var song = db.Songs.Include(s => s.Artist).Include(s => s.Genres).FirstOrDefault(s => s.Id == songId);

    if (song == null)
    {
        return Results.NotFound(songId);
    }

    return Results.Ok();
});

// Get all Songs and relative Artist and Genre
app.MapGet("/api/songs/{songId}", (TominoTrumpetsDbContext db, int id) =>
{
    var SongArtistAndGenre = db.Songs.Include(s => s.Artist).Include(s => s.Genres).FirstOrDefault(s => s.Id == id);

    if (SongArtistAndGenre == null) 
    {
        return Results.NotFound();
    }

    return Results.Ok();
});

// Artist Api Calls

// Create an Artist
app.MapPost("/api/artists", (TominoTrumpetsDbContext db, Artist newArtist) =>
{
    db.Artists.Add(newArtist);
    db.SaveChanges();
    return Results.Created($"/api/artists/{newArtist.Id}", newArtist);
});

// Delete an Artist
app.MapDelete("/api/artists/{artistId}", (TominoTrumpetsDbContext db, int artistId) =>
{
    var artist = db.Artists.SingleOrDefault(a => a.Id == artistId);
    if(artist == null)
    {
        return Results.NotFound();
    }
    db.Artists.Remove(artist);
    db.SaveChanges();
    return Results.NoContent();
});

// Update an Artist
app.MapPut("/api/artists/{artistId}", (TominoTrumpetsDbContext db, Artist artist, int artistId ) =>
{
    var updateArtist = db.Artists.SingleOrDefault(a => a.Id == artistId);
    if (updateArtist == null)
    {
        return Results.NotFound();
    }
    updateArtist.Name = artist.Name;
    updateArtist.Age = artist.Age;
    updateArtist.Bio = artist.Bio;

    db.SaveChanges();
    return Results.Created($"/api/artists/artist.id", artist);
});

// View a List of All the Artists
app.MapGet("/api/artists", (TominoTrumpetsDbContext db) =>
{
   return db.Artists.ToList();
});

// Genre Api Calls

// Get all Genres
app.MapGet("/api/genres", (TominoTrumpetsDbContext db) =>
{
    return db.Genres.ToList();
});

// Create a New Genre
app.MapPost("/api/genres", (TominoTrumpetsDbContext db, Genre newGenre) =>
{
    db.Genres.Add(newGenre);
    db.SaveChanges();
    return Results.Created($"/api/genre/{newGenre.Id}", newGenre);
});

// Update a Genre
app.MapPut("/api/genres/{genreId}", (TominoTrumpetsDbContext db, Genre genre, int id) =>
{
    Genre genreUpdate = db.Genres.SingleOrDefault(g => genre.Id == id);
    if (genreUpdate == null) 
    {
        return Results.NotFound();
    }
    genreUpdate.Description = genre.Description;
    db.SaveChanges();
    return Results.Created($"/api/genres/{genre.Id}", genre);
});

// Delete a Genre
app.MapDelete("/api/genres/{genreId}", (TominoTrumpetsDbContext db, int id) =>
{
    var genre = db.Genres.SingleOrDefault(g => g.Id == id);
    db.Genres.Remove(genre);
    db.SaveChanges();
    return Results.NoContent();
});

// Get all the Songs related to a Genre
app.MapGet("/api/genres/{genreId}", (TominoTrumpetsDbContext db, int id) =>
{
    var SongsWithGenreId = db.Genres.Where(s => s.Id == id).Include(g => g.Songs).ToList();

    if (SongsWithGenreId == null)
    {
        return Results.NotFound();
    }

    return Results.Ok();
});
app.Run();

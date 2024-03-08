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

// Create a Song (Post)
app.MapPost("/api/songs", (TominoTrumpetsDbContext db, int id, string title, int artistId, string album, int length) =>
{
    var newSong = new Song
    {
        Id = id,
        Title = title,
        ArtistId = artistId,
        Album = album,
        Length = length
    };

    db.Songs.Add(newSong);
    db.SaveChanges();
    return Results.Created($"/api/songs/{newSong.Id}", newSong);
});

//Delete a Song (Delete)
app.MapDelete("/api/songs/{songId}", (TominoTrumpetsDbContext db, int songId) =>
{
    var song = db.Songs.SingleOrDefault(s => s.Id == songId);

    if(song == null)
    {
        return Results.NotFound();
    }

    db.Songs.Remove(song);
    db.SaveChanges();
    return Results.NoContent();
});

//Update a Song (Put)
app.MapPut("/api/songs/{songId}", (TominoTrumpetsDbContext db, int artistId, string title, string album, int length ) =>
{
    var updateSong = db.Songs.SingleOrDefault(s => s.Id == artistId);
    if (updateSong == null)
    {
        return Results.NotFound();
    }
    updateSong.Title = title;
    updateSong.ArtistId = artistId;
    updateSong.Album = album;
    updateSong.Length = length;

    db.SaveChanges();
    return Results.Ok();
});

//View a List of all the Songs
app.MapGet("/api/songs", (TominoTrumpetsDbContext db) =>
{
    var songs = db.Songs.ToList();

    if (songs == null || songs.Count == 0) 
    {
        return Results.NotFound();
    }


    var response = songs.Select(song => new
    {
        id = song.Id,
        title = song.Title,
        album = song.Album,
        length = song.Length,
    });

    return Results.Ok();
    
});

//View all the Songs by Id
app.MapGet("/api/sons/{songId}", (TominoTrumpetsDbContext db, int songId) =>
{
    var song = db.Songs
    .Include(s => s.Artist)
    .Include(s => s.Genres)
    .FirstOrDefault(s => s.Id == songId);

    if (song == null)
    {
        return Results.NotFound(songId);
    }

    var response = new
    {
        id = song.Id,
        title = song.Title,
        artist = new
        {
            id = song.Artist.Id,
            name = song.Artist.Name,
            age = song.Artist.Age,
            bio = song.Artist.Bio
        },
        album = song.Album,
        length = song.Length,
        genre = song.Genres.Select(genre => new
        {
            id = genre.Id,
            description = genre.Description,
        }).ToList()
    };

    return Results.Ok();
});


// Create an Artist
app.MapPost("/api/artists", (TominoTrumpetsDbContext db, int id, string name, int age, string bio) =>
{
    var newArtist = new Artist
    {
        Id = id,
        Name = name,
        Age = age,
        Bio = bio
    };

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
app.MapPut("/api/artists/{artistId}", (TominoTrumpetsDbContext db, int artistId, string name, int age, string bio ) =>
{
    var updateArtist = db.Artists.SingleOrDefault(a => a.Id == artistId);
    if (updateArtist == null)
    {
        return Results.NotFound();
    }
    updateArtist.Name = name;
    updateArtist.Age = age;
    updateArtist.Bio = bio;

    db.SaveChanges();
    return Results.Ok();
});

// View a List of All the Artists
app.MapGet("/api/artists", (TominoTrumpetsDbContext db) =>
{
    var artists = db.Artists.ToList();

    if (artists == null || artists.Count == 0)
    {
        return Results.NotFound();
    }

    var response = artists.Select(artist => new
    {
        id = artist.Id,
        name = artist.Name,
        age = artist.Age,
        bio = artist.Bio,
    });

    return Results.Ok();
});
app.Run();

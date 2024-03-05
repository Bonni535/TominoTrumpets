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
app.MapPost("/api/songs", (TominoTrumpetsDbContext db, Song newSong) =>
{

    db.Songs.Add(newSong);
    db.SaveChanges();
    return Results.Created($"/api/Songs/{newSong.Id}", newSong);
});

//Delete a Song (Delete)
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


app.Run();

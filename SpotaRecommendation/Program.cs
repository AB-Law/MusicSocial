using Microsoft.EntityFrameworkCore;
using SpotaRecommendation.Data;
using SpotaRecommendation.Helpers;
using SpotaRecommendation.Services.Implementation;
using SpotaRecommendation.Services.Interface;

var builder = WebApplication.CreateBuilder(args);

// ✅ Register dependencies
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHttpClient<SpotifyClient>(); // for SpotifyClient
builder.Services.AddHttpClient<ISpotifyAuthService, SpotifyAuthService>();
builder.Services.AddScoped<ISpotifyAuthService, SpotifyAuthService>(); // if you use an auth service
builder.Services.AddHttpClient<ISpotifyPlaylistService, SpotifyPlaylistService>();
builder.Services.AddControllers(); // required!

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
using System.Net.Http.Headers;
using System.Text.Json;
using SpotaRecommendation.Models;
using SpotaRecommendation.Services.Interface;
using SpotaRecommendation.Data; // Make sure this is imported if ApplicationDbContext is there
using Microsoft.EntityFrameworkCore;

namespace SpotaRecommendation.Services.Implementation
{
    public class SpotifyPlaylistService : ISpotifyPlaylistService
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _dbContext;

        public SpotifyPlaylistService(HttpClient httpClient, AppDbContext dbContext)
        {
            _httpClient = httpClient;
            _dbContext = dbContext;
        }

        public async Task<List<SpotifyPlaylistDto>> GetUserPlaylistsAsync(string accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.spotify.com/v1/me/playlists");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var playlists = JsonSerializer.Deserialize<SpotifyPlaylistResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return playlists?.Items ?? new List<SpotifyPlaylistDto>();
        }

        public async Task SavePlaylistsToDbAsync(List<SpotifyPlaylistDto> spotifyPlaylists, User user)
        {
            var existingIds = await _dbContext.Playlists
                .Where(p => p.UserId == user.Id)
                .Select(p => p.SpotifyPlaylistId)
                .ToListAsync();

            foreach (var dto in spotifyPlaylists)
            {
                if (!existingIds.Contains(dto.Id))
                {
                    _dbContext.Playlists.Add(new Playlist
                    {
                        SpotifyPlaylistId = dto.Id,
                        Name = dto.Name,
                        UserId = user.Id,
                        IsRecommended = false
                    });
                }
            }

            await _dbContext.SaveChangesAsync();
        }
        
        
        public async Task<List<SpotifyTrackDto>> GetTracksFromPlaylistAsync(string accessToken, string playlistId)
        {
            var result = new List<SpotifyTrackDto>();

            if (string.IsNullOrWhiteSpace(playlistId))
                throw new ArgumentException("Playlist ID cannot be null or empty.");

            var url = $"https://api.spotify.com/v1/playlists/{Uri.EscapeDataString(playlistId)}/tracks";
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Spotify API Error {response.StatusCode}: {json}");
                throw new HttpRequestException($"Spotify API error: {json}");
            }

            using var doc = JsonDocument.Parse(json);
            foreach (var item in doc.RootElement.GetProperty("items").EnumerateArray())
            {
                var track = item.GetProperty("track");
                var albumImages = track.GetProperty("album").GetProperty("images");
                string imageUrl = albumImages.GetArrayLength() > 0
                    ? albumImages[0].GetProperty("url").GetString()
                    : null;


                result.Add(new SpotifyTrackDto
                {
                    Id = track.GetProperty("id").GetString(),
                    Name = track.GetProperty("name").GetString(),
                    Album = track.GetProperty("album").GetProperty("name").GetString(),
                    Artist = track.GetProperty("artists")[0].GetProperty("name").GetString(),
                    ImageUrl = imageUrl
                });
            }

            return result;
        }

    
    }
}

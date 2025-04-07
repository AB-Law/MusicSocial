using System.Net.Http.Headers;
using System.Text.Json;
using SpotaRecommendation.Data;
using SpotaRecommendation.Helpers;
using SpotaRecommendation.Models;
using SpotaRecommendation.Services.Interface;

namespace SpotaRecommendation.Services.Implementation;

public class SpotifyAuthService : ISpotifyAuthService
{
    private readonly IConfiguration _config;
    private readonly SpotifyClient _client;
    private readonly HttpClient _httpClient;
    private readonly AppDbContext _dbContext;

    public SpotifyAuthService(IConfiguration config, SpotifyClient client, HttpClient httpClient, AppDbContext dbContext)
    {
        _dbContext = dbContext;
        _config = config;
        _client = client;
        _httpClient = httpClient;
    }

    public string GetLoginUrl()
    {
        var clientId = _config["Spotify:ClientId"];
        var redirectUri = _config["Spotify:RedirectUri"];
        var scopes = "playlist-read-private playlist-read-collaborative playlist-modify-public playlist-modify-private";
        var url = $"https://accounts.spotify.com/authorize" +
                  $"?client_id={clientId}" +
                  $"&response_type=code" +
                  $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                  $"&scope={Uri.EscapeDataString(scopes)}";
        return url;
    }

    public async Task<SpotifyTokenResponse> ExchangeCodeForToken(string code)
    {
        return await _client.GetTokensAsync(code);
    }

    public async Task<SpotifyUserProfile?> GetSpotifyUserProfile(string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.spotify.com/v1/me");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            return null;

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<SpotifyUserProfile>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
}
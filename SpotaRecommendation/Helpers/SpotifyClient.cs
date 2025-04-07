namespace SpotaRecommendation.Helpers;

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using SpotaRecommendation.Models;


public class SpotifyClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public SpotifyClient(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<SpotifyTokenResponse> GetTokensAsync(string code)
    {
        var clientId = _config["Spotify:ClientId"];
        var clientSecret = _config["Spotify:ClientSecret"];
        var redirectUri = _config["Spotify:RedirectUri"];

        var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));

        var postData = new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "code", code },
            { "redirect_uri", redirectUri }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token")
        {
            Headers = {
                Authorization = new AuthenticationHeaderValue("Basic", authHeader)
            },
            Content = new FormUrlEncodedContent(postData)
        };

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var tokens = JsonSerializer.Deserialize<SpotifyTokenResponse>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return tokens!;
    }
}

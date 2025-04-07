using Microsoft.EntityFrameworkCore;
using SpotaRecommendation.Data;
using SpotaRecommendation.Models;
using SpotaRecommendation.Services.Interface;

namespace SpotaRecommendation.Controllers;

using Microsoft.AspNetCore.Mvc;
using SpotaRecommendation.Services;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly ISpotifyAuthService _spotifyAuthService;
    private readonly AppDbContext _dbContext;

    public AuthController(ISpotifyAuthService spotifyAuthService, AppDbContext dbContext)
    {
        _dbContext = dbContext;
        _spotifyAuthService = spotifyAuthService;
    }

    [HttpGet("login")]
    public IActionResult Login()
    {
        var authUrl = _spotifyAuthService.GetLoginUrl();
        return Redirect(authUrl);
    }

    [HttpGet("callback")]
    public async Task<IActionResult> Callback([FromQuery] string code)
    {
        var tokens = await _spotifyAuthService.ExchangeCodeForToken(code);

        if (tokens == null)
            return BadRequest("Token exchange failed");

        // Get user profile from Spotify
        var profile = await _spotifyAuthService.GetSpotifyUserProfile(tokens.AccessToken);

        if (profile == null)
            return BadRequest("Failed to fetch Spotify user profile");

        // Check if user exists
        var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.SpotifyId == profile.Id);

        if (existingUser != null)
        {
            // Update tokens
            existingUser.AccessToken = tokens.AccessToken;
            existingUser.RefreshToken = tokens.RefreshToken;
            existingUser.TokenExpiry = DateTime.UtcNow.AddSeconds(tokens.ExpiresIn);
        }
        else
        {
            // Create new user
            var newUser = new User
            {
                SpotifyId = profile.Id,
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                TokenExpiry = DateTime.UtcNow.AddSeconds(tokens.ExpiresIn)
            };

            _dbContext.Users.Add(newUser);
        }

        await _dbContext.SaveChangesAsync();

        return Ok(new
        {
            access_token = tokens.AccessToken,
            refresh_token = tokens.RefreshToken,
            expires_in = tokens.ExpiresIn,
            token_type = tokens.TokenType
        });
    }


}

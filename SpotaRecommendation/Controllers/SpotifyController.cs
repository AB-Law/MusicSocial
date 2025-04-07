using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpotaRecommendation.Data;
using SpotaRecommendation.Services.Interface;

namespace SpotaRecommendation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpotifyController : ControllerBase
{
    private readonly ISpotifyPlaylistService _playlistService;
    private readonly AppDbContext _dbContext;

    public SpotifyController(ISpotifyPlaylistService playlistService, AppDbContext dbContext)
    {
        _playlistService = playlistService;
        _dbContext = dbContext;
    }

    [HttpGet("playlists")]
    public async Task<IActionResult> GetPlaylists([FromQuery] string accessToken)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.AccessToken == accessToken);
        
        if (user == null)
            return NotFound("User not found");

        var playlists = await _playlistService.GetUserPlaylistsAsync(accessToken);
        await _playlistService.SavePlaylistsToDbAsync(playlists, user);
        
        return Ok(playlists);
    }
    
    [HttpGet("playlist-tracks")]
    public async Task<IActionResult> GetTracks([FromQuery] string accessToken, [FromQuery] string playlistId)
    {
        var tracks = await _playlistService.GetTracksFromPlaylistAsync(accessToken, playlistId);
        return Ok(tracks);
    }

}
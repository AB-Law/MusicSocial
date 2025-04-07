using SpotaRecommendation.Models;

namespace SpotaRecommendation.Services.Interface;

public interface ISpotifyPlaylistService
{
    Task<List<SpotifyPlaylistDto>> GetUserPlaylistsAsync(string accessToken);
    Task SavePlaylistsToDbAsync(List<SpotifyPlaylistDto> spotifyPlaylists, User user);
    
    Task<List<SpotifyTrackDto>> GetTracksFromPlaylistAsync(string accessToken, string playlistId);

}

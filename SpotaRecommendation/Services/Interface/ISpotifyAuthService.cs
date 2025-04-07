using SpotaRecommendation.Models;

namespace SpotaRecommendation.Services.Interface;

public interface ISpotifyAuthService
{
    string GetLoginUrl();
    Task<SpotifyTokenResponse> ExchangeCodeForToken(string code);
    Task<SpotifyUserProfile?> GetSpotifyUserProfile(string accessToken);

}


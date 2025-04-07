namespace SpotaRecommendation.Models
{
    public class User
    {
        public int Id { get; set; }
        public string SpotifyId { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime TokenExpiry { get; set; }

        public ICollection<Playlist> Playlists { get; set; }
    }
}
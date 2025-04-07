namespace SpotaRecommendation.Models
{
    public class Playlist
    {
        public int Id { get; set; }
        public string SpotifyPlaylistId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsRecommended { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
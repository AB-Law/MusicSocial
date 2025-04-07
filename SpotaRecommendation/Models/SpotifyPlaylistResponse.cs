namespace SpotaRecommendation.Models;

public class SpotifyPlaylistResponse
{
    public List<SpotifyPlaylistDto> Items { get; set; }
}

public class SpotifyPlaylistDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public TracksInfo Tracks { get; set; }
    public List<ImageInfo> Images { get; set; }
}

public class TracksInfo
{
    public int Total { get; set; }
}

public class ImageInfo
{
    public string Url { get; set; }
}

public class SpotifyTrackDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Album { get; set; }
    public string Artist { get; set; }
    public string ImageUrl { get; set; } // 👈 add this
}





using PlaylistRecommenderAPI.Models;
public interface IMusicService
{
    Task<TrackInfo> SearchMusic(string name);
    Task<List<string>> GetGenresFromTracks(List<string> songNames);
    Task<string?> GetArtistIdFromTrack(string songName);
    Task<List<string>> GetArtistGenres(string artistId);
    string? FindMostCommonGenre(List<string> genres);
}
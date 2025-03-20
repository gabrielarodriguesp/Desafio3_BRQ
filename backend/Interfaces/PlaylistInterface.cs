public interface IPlaylistService
{
    Task<string?> RecommendPlaylist(List<string> songNames);
    Task<string?> GetPlaylistByGenre(string genre);
    Task<object?> GetPlaylistDetails(string playlistId);
    Task<(string Name, string ImageUrl)?> GetPlaylistInfo(string playlistUrl);
    string ExtractPlaylistIdFromUrl(string playlistUrl);
}
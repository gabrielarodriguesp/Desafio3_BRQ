public class UserSearch
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string SearchedSongs { get; set; } = string.Empty;
    public string? RecommendedPlaylist { get; set; }
    public DateTime? SearchDate { get; set; }
}

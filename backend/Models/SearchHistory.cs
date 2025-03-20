using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

public class SearchHistory
{
    [Column ("id")]
    public int Id { get; set; }
    [Column("email")]
    public string? Email { get; set; }
    [Column("searched_songs")]
    public List<string> SearchedSongs { get; set; } = new List<string>();
    [Column("recommended_playlist")]
    public string? RecommendedPlaylist { get; set; }
    [Column("search_date")]
    public DateTime SearchDate { get; set; } = DateTime.UtcNow;
}

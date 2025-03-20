namespace PlaylistRecommenderAPI.Models;

public class SearchRequest
{
    public string Email { get; set; } = string.Empty;
    public List<string>? Songs { get; set; }
}

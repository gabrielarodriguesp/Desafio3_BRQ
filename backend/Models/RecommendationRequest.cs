namespace PlaylistRecommenderAPI.Models;

public class RecommendationRequest
{
    public string Email { get; set; } = string.Empty;
    public string PlaylistUrl { get; set; } = string.Empty;
}

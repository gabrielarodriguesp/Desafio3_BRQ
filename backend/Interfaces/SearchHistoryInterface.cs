using Microsoft.AspNetCore.Mvc;
using PlaylistRecommenderAPI.Models;

public interface ISearchHistoryService
{
    Task<IActionResult> RegisterSearch(SearchRequest request);
    Task<IActionResult> SaveRecommendation(RecommendationRequest request);
    Task<IActionResult> GetSearchHistory(string email);
}
using Microsoft.AspNetCore.Mvc;
using PlaylistRecommenderAPI.Models;

[ApiController]
[Route("api/searchhistory")]
public class SearchHistoryController : ControllerBase
{
    private readonly ISearchHistoryService _searchHistoryService;

    public SearchHistoryController(ISearchHistoryService searchHistoryService)
    {
        _searchHistoryService = searchHistoryService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterSearch([FromBody] SearchRequest request)
    {
        var result = await _searchHistoryService.RegisterSearch(request);
        return result;
    }

    [HttpPost("recommendation")]
    public async Task<IActionResult> SaveRecommendation([FromBody] RecommendationRequest request)
    {
        return await _searchHistoryService.SaveRecommendation(request);
    }

    [HttpGet("history/{email}")]
    public async Task<IActionResult> GetHistory(string email)
    {
        return await _searchHistoryService.GetSearchHistory(email);
    }
}


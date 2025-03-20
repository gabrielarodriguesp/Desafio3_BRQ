using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlaylistRecommenderAPI.Models;

namespace PlaylistRecommenderAPI.Services
{
    public class SearchHistoryService : ISearchHistoryService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;
        private readonly IPlaylistService _playlistService;

        public SearchHistoryService(AppDbContext context, IConfiguration configuration, HttpClient client, IPlaylistService playlistService)
        {
            _context = context;
            _configuration = configuration;
            _client = client;
            _playlistService = playlistService;
        }

        public async Task<IActionResult> RegisterSearch(SearchRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || request.Songs == null || !request.Songs.Any())
            {
                return new BadRequestObjectResult(new ApiResponse { Message = "Dados inválidos. O email e pelo menos uma música são obrigatórios." });
            }

            try
            {
                var songNames = request.Songs.Select(s => s.ToString()).ToList();
                var recommendedPlaylist = await _playlistService.RecommendPlaylist(songNames) ?? string.Empty;

                var searchHistory = new SearchHistory
                {
                    Email = request.Email,
                    SearchedSongs = songNames,
                    RecommendedPlaylist = recommendedPlaylist,
                    SearchDate = DateTime.UtcNow
                };

                _context.SearchHistory.Add(searchHistory);
                await _context.SaveChangesAsync();
                _context.ChangeTracker.Clear();

                return new OkObjectResult(new ApiResponse { Message = "Pesquisa registrada com sucesso!", RecommendedPlaylist = recommendedPlaylist });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar no banco: {ex.Message}");
                return new StatusCodeResult(500);
            }
        }

        public async Task<IActionResult> SaveRecommendation(RecommendationRequest request)
        {
            var searchHistory = await _context.SearchHistory
                .OrderByDescending(s => s.Id)
                .FirstOrDefaultAsync(s => s.Email == request.Email);

            if (searchHistory == null)
                return new NotFoundObjectResult("Histórico de pesquisa não encontrado!");

            searchHistory.RecommendedPlaylist = request.PlaylistUrl;
            searchHistory.SearchDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return new OkObjectResult(new { message = "Pesquisa registrada com sucesso!" });
        }

        public async Task<IActionResult> GetSearchHistory(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return new BadRequestObjectResult("Email inválido.");
            }

            try
            {
                var history = await _context.SearchHistory
                    .Where(s => s.Email == email)
                    .OrderByDescending(s => s.SearchDate)
                    .Take(10)
                    .ToListAsync();

                if (!history.Any())
                {
                    return new NotFoundObjectResult("Nenhum histórico encontrado para este email.");
                }

                var historyWithImages = new List<object>();
                foreach (var entry in history)
                {
                    if (!string.IsNullOrEmpty(entry.RecommendedPlaylist))
                    {
                        var playlistInfo = await _playlistService.GetPlaylistInfo(entry.RecommendedPlaylist);
                        historyWithImages.Add(new
                        {
                            Name = playlistInfo?.Name ?? "Playlist desconhecida",
                            ImageUrl = playlistInfo?.ImageUrl ?? "default_playlist_image_url",
                            Link = entry.RecommendedPlaylist
                        });
                    }
                }

                return new OkObjectResult(historyWithImages);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar histórico: {ex.Message}");
                return new ObjectResult("Erro ao buscar histórico.") { StatusCode = 500 };
            }
        }
    }
}
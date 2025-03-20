using Microsoft.AspNetCore.Mvc;

[Route("api/playlist")]
[ApiController]
public class PlaylistController : ControllerBase
{
    private readonly IUserService _spotifyService;
    private readonly IPlaylistService _playlistService;

    public PlaylistController(IUserService spotifyService, IPlaylistService playlistService)
    {
        _spotifyService = spotifyService;
        _playlistService = playlistService;
    }

    [HttpPost("recommend")]
    public async Task<IActionResult> RecommendPlaylist([FromBody] List<string> songNames)
    {
        try
        {
            if (songNames == null || songNames.Count == 0 || songNames.Count > 5)
                return BadRequest("Envie entre 1 e 5 músicas.");

            var playlistUrl = await _playlistService.RecommendPlaylist(songNames);
            if (string.IsNullOrEmpty(playlistUrl))
            {
                Console.WriteLine("Nenhuma playlist pode ser gerada pois não há gênero comum.");
                return BadRequest("Não foi possível encontrar um gênero comum entre as músicas fornecidas.");
            }

            return Ok(new PlaylistResponse { PlaylistUrl = playlistUrl });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro no RecommendPlaylist: {ex.Message}");
            return StatusCode(500, "Erro interno no servidor.");
        }
    }

    [HttpGet("details")]
    public async Task<IActionResult> GetPlaylistDetails([FromQuery] string id)
    {
        try
        {
            var playlistDetails = await _playlistService.GetPlaylistDetails(id);
            if (playlistDetails == null)
                return NotFound("Detalhes da playlist não encontrados.");

            return Ok(playlistDetails);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar detalhes da playlist: {ex.Message}");
            return StatusCode(500, "Erro interno no servidor.");
        }
    }
}

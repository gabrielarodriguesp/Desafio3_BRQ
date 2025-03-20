using Microsoft.AspNetCore.Mvc;

namespace PlaylistRecommenderAPI.Controllers
{
    [Route("api/music")]
    [ApiController]
    public class MusicController : ControllerBase
    {
        private readonly IMusicService _musicService;

        public MusicController(IMusicService musicService)
        {
            _musicService = musicService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchMusic([FromQuery] string name)
        {
            var track = await _musicService.SearchMusic(name);
            if (track == null)
                return NotFound("Música não encontrada.");

            return Ok(track);
        }
    }
}

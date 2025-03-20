using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using PlaylistRecommenderAPI.Models;

namespace PlaylistRecommenderAPI.Services
{
    public class MusicService : IMusicService
    {
        private readonly IUserService _userService;
        private readonly HttpClient _httpClient;

        public MusicService(IUserService userService, HttpClient httpClient)
        {
            _userService = userService;
            _httpClient = httpClient;
        }

        public virtual async Task<TrackInfo> SearchMusic(string songName)
        {
            if (string.IsNullOrWhiteSpace(songName))
            {
                return null!;
            }

            var accessToken = await _userService.EnsureValidToken();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.GetAsync($"https://api.spotify.com/v1/search?q={Uri.EscapeDataString(songName)}&type=track&limit=1");

            if (!response.IsSuccessStatusCode) return null!;

            var json = JObject.Parse(await response.Content.ReadAsStringAsync());
            var track = json["tracks"]?["items"]?.FirstOrDefault();
            if (track == null) return null!;

            return new TrackInfo
            {
                Name = track["name"]?.ToString() ?? "",
                Artist = track["artists"]?.FirstOrDefault()?["name"]?.ToString() ?? "",
                AlbumCover = track["album"]?["images"]?.FirstOrDefault()?["url"]?.ToString() ?? ""
            };
        }

        public async Task<List<string>> GetGenresFromTracks(List<string> songNames)
        {
            if (songNames == null || !songNames.Any())
            {
                return new List<string>();
            }

            var genres = new List<string>();

            foreach (var songName in songNames)
            {
                var artistId = await GetArtistIdFromTrack(songName);
                if (!string.IsNullOrEmpty(artistId))
                {
                    var artistGenres = await GetArtistGenres(artistId);
                    genres.AddRange(artistGenres);
                }
            }

            return genres;
        }

        public virtual async Task<string?> GetArtistIdFromTrack(string songName)
        {
            if (string.IsNullOrWhiteSpace(songName))
            {
                return null;
            }

            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await _userService.EnsureValidToken());

            var response = await client.GetAsync($"https://api.spotify.com/v1/search?q={Uri.EscapeDataString(songName)}&type=track&limit=1");
            if (!response.IsSuccessStatusCode) return null;

            var json = JObject.Parse(await response.Content.ReadAsStringAsync());
            return json["tracks"]?["items"]?.FirstOrDefault()?["artists"]?.FirstOrDefault()?["id"]?.ToString();
        }

        public virtual async Task<List<string>> GetArtistGenres(string artistId)
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await _userService.EnsureValidToken());

            var response = await client.GetAsync($"https://api.spotify.com/v1/artists/{artistId}");
            if (!response.IsSuccessStatusCode) return new List<string>();

            var json = JObject.Parse(await response.Content.ReadAsStringAsync());
            return json["genres"]?.Select(g => g.ToString()).ToList() ?? new List<string>();
        }

        public virtual string? FindMostCommonGenre(List<string> genres)
        {
            Console.WriteLine("Lista de gêneros em Comum: " + string.Join(", ", genres));

            if (genres == null || !genres.Any())
            {
                Console.WriteLine("Nenhum gênero encontrado.");
                return null;
            }

            return genres.GroupBy(g => g)
                         .OrderByDescending(g => g.Count())
                         .Select(g => g.Key)
                         .FirstOrDefault();

        }
    }
}

using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace PlaylistRecommenderAPI.Services
{
    public class PlaylistService : IPlaylistService
    {

        private readonly IUserService _spotifyService;
        private readonly IMusicService _musicService;
        private readonly HttpClient _httpClient;

        public PlaylistService(IUserService spotifyService, IMusicService musicService, HttpClient httpClient)
        {
            _spotifyService = spotifyService;
            _musicService = musicService;
            _httpClient = httpClient;
        }

        public virtual async Task<string?> RecommendPlaylist(List<string> songNames)
        {
            var genres = await _musicService.GetGenresFromTracks(songNames);

            if (genres.Count == 0)
            {
                Console.WriteLine("Nenhum gênero encontrado para as músicas fornecidas.");
                return null;
            }

            var commonGenre = _musicService.FindMostCommonGenre(genres);

            if (string.IsNullOrWhiteSpace(commonGenre))
            {
                Console.WriteLine("Nenhum gênero comum encontrado entre as músicas.");
                return null;
            }

            return await GetPlaylistByGenre(commonGenre);
        }


        public async Task<string?> GetPlaylistByGenre(string genre)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await _spotifyService.EnsureValidToken());

            var response = await _httpClient.GetAsync($"https://api.spotify.com/v1/search?q={Uri.EscapeDataString(genre)}&type=playlist&limit=1");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Retornando null pois a requisição falhou.");
                return null;
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Resposta JSON da API do Spotify: {jsonString}");
            var json = JObject.Parse(jsonString);

            if (json["playlists"] == null || json["playlists"]!["items"] == null)
            {
                Console.WriteLine("Nenhuma playlist encontrada.");
                return null;
            }

            var playlistsToken = json["playlists"]!["items"] ?? string.Empty;

            if (playlistsToken.Type != JTokenType.Array || !playlistsToken.HasValues)
            {
                Console.WriteLine("Nenhuma playlist válida encontrada.");
                return null;
            }

            var playlists = (JArray)playlistsToken;
            var playlist = playlists.FirstOrDefault();

            if (playlist == null)
            {
                Console.WriteLine("Nenhuma playlist válida encontrada.");
                return null;
            }

            if (playlist["external_urls"] is JObject externalUrls && externalUrls.ContainsKey("spotify"))
            {
                return externalUrls["spotify"]?.ToString();
            }

            Console.WriteLine("Playlist encontrada, mas sem URL.");
            return null;
        }


        public async Task<object?> GetPlaylistDetails(string playlistId)
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await _spotifyService.EnsureValidToken());

            var response = await client.GetAsync($"https://api.spotify.com/v1/playlists/{playlistId}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Falha ao obter detalhes da playlist.");
                return null;
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(jsonString);

            return new
            {
                name = json["name"]?.ToString(),
                imageUrl = json["images"]?.FirstOrDefault()?["url"]?.ToString()
            };
        }

        public async Task<(string Name, string ImageUrl)?> GetPlaylistInfo(string playlistUrl)
        {
            if (string.IsNullOrWhiteSpace(playlistUrl)) return null;

            var playlistId = ExtractPlaylistIdFromUrl(playlistUrl);
            if (string.IsNullOrWhiteSpace(playlistId)) return null;

            var accessToken = await _spotifyService.EnsureValidToken();
            if (string.IsNullOrWhiteSpace(accessToken)) return null;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.GetAsync($"https://api.spotify.com/v1/playlists/{playlistId}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<JsonElement>(json);

            var name = data.GetProperty("name").GetString() ?? string.Empty;
            var imageUrl = data.GetProperty("images")[0].GetProperty("url").GetString() ?? string.Empty;

            return (name, imageUrl);
        }

        public string ExtractPlaylistIdFromUrl(string playlistUrl)
        {
            var match = Regex.Match(playlistUrl, @"playlist/([a-zA-Z0-9]+)");
            return match.Success ? match.Groups[1].Value : string.Empty;
        }
    }
}
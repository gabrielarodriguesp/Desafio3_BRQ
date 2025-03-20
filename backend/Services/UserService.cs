using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace PlaylistRecommenderAPI.Services
{
    public class UserService : IUserService
    {
        private readonly string clientId;
        private readonly string clientSecret;
        private string? accessToken;
        private readonly HttpClient client;

        public UserService(IConfiguration config, HttpClient httpClient)
        {
            clientId = config["Spotify:ClientId"] ?? throw new ArgumentNullException(nameof(config), "ClientId é nulo.");
            clientSecret = config["Spotify:ClientSecret"] ?? throw new ArgumentNullException(nameof(config), "ClientSecret é nulo.");
            client = httpClient;
        }

        public virtual async Task<string> EnsureValidToken()
        {
            if (accessToken == null)
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(clientId + ":" + clientSecret)));
                request.Content = new FormUrlEncodedContent(new Dictionary<string, string> { { "grant_type", "client_credentials" } });

                var response = await client.SendAsync(request);
                var json = JObject.Parse(await response.Content.ReadAsStringAsync());
                accessToken = json["access_token"]?.ToString();
            }
            return accessToken!;
        }

    }
}

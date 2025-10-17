using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MovieInfoSender.MovieInfoSender;

namespace MovieInfoSender
{
    public class OmdbService
    {
        private readonly string _apiKey;
        private readonly HttpClient _client = new HttpClient();

        public OmdbService(string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task<MovieInfo?> GetMovieInfoAsync(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return null;

            var url = $"https://www.omdbapi.com/?t={Uri.EscapeDataString(title)}&apikey={_apiKey}";
            var response = await _client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("Response", out var ok) || ok.GetString() != "True")
                return null;

            MovieInfo movie = new()
            {
                Title = Get(doc, "Title"),
                Year = Get(doc, "Year"),
                Genre = Get(doc, "Genre"),
                Director = Get(doc, "Director"),
                Actors = Get(doc, "Actors"),
                Plot = Get(doc, "Plot"),
                Poster = Get(doc, "Poster")
            };

            return movie;
        }

        private static string Get(JsonDocument doc, string name)
        {
            if (doc.RootElement.TryGetProperty(name, out var value))
            {
                var str = value.GetString();
                return (str == "N/A") ? string.Empty : str;
            }
            return string.Empty;
        }
    }
}

using Newtonsoft.Json; // dotnet add package Newtonsoft.Json
using RestSharp; // dotnet add package RestSharp
using System.Drawing;
using Console = Colorful.Console; // dotnet add package Colorful.Console

class Movie
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("title")]
    public string? Title { get; set; }

    [JsonProperty("overview")]
    public string? Overview { get; set; }

    [JsonProperty("release_date")]
    public string? ReleaseDate { get; set; }

    [JsonProperty("vote_average")]
    public double? VoteAverage { get; set; }

    [JsonProperty("vote_count")]
    public int? VoteCount { get; set; }
}

class MovieResponse
{
    [JsonProperty("results")]
    public List<Movie>? Results { get; set; }
}

class Credit
{
    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("character")]
    public string? Character { get; set; }

    [JsonProperty("job")]
    public string? Job { get; set; }
}

class CreditResponse
{
    [JsonProperty("cast")]
    public List<Credit>? Cast { get; set; }

    [JsonProperty("crew")]
    public List<Credit>? Crew { get; set; }
}

class MovieApi
{
    private readonly RestClient client;
    private const string ApiKey = "8f663caefb9c78b4b33f1c2ff31d13f3";

    public MovieApi(string baseUrl)
    {
        client = new RestClient(baseUrl);
    }

    public async Task<List<Movie>?> GetTopRatedMoviesAsync()
    {
        var movies = new List<Movie>();

        for (int page = 1; page <= 10; page++)
        {
            var request = new RestRequest("movie/top_rated", Method.Get);
            request.AddHeader("Accept", "application/json");
            request.AddParameter("api_key", ApiKey);
            request.AddParameter("language", "uk-UA");
            request.AddParameter("page", page);

            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                try
                {
                    var movieResponse = JsonConvert.DeserializeObject<MovieResponse>(response.Content ?? "");
                    if (movieResponse?.Results != null)
                    {
                        movies.AddRange(movieResponse.Results);
                        if (movies.Count >= 100) break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка парсингу JSON: {ex.Message}", Color.Red);
                    Console.WriteLine("Відповідь сервера:");
                    Console.WriteLine(response.Content);
                }
            }
            else
            {
                Console.WriteLine($"Помилка запиту: {response.StatusCode} - {response.StatusDescription}", Color.Red);
                Console.WriteLine("Відповідь сервера:");
                Console.WriteLine(response.Content);
            }
        }

        return movies.Take(100).ToList();
    }

    public async Task<(List<string> Actors, string Director, string Writer, string Composer)> GetMovieCreditsAsync(int movieId)
    {
        var request = new RestRequest($"movie/{movieId}/credits", Method.Get);
        request.AddParameter("api_key", ApiKey);
        request.AddParameter("language", "uk-UA");

        var response = await client.ExecuteAsync<CreditResponse>(request);
        if (!response.IsSuccessful || response.Data == null)
        {
            return (new List<string> { "Список акторів недоступний" }, "Невідомо", "Невідомо", "Невідомо");
        }

        var actors = response.Data.Cast != null
            ? response.Data.Cast
                .Take(5)
                .Select(a => $"{a.Name} ({a.Character ?? "персонаж невідомий"})")
                .ToList()
            : new List<string> { "Список акторів недоступний" };

        var director = response.Data.Crew?.FirstOrDefault(c => c.Job == "Director")?.Name ?? "Невідомо";

        var writer = response.Data.Crew?.FirstOrDefault(c => c.Job == "Writer" || c.Job == "Screenplay")?.Name ?? "Невідомо";

        var composer = response.Data.Crew?.FirstOrDefault(c => c.Job == "Original Music Composer")?.Name ?? "Невідомо";

        return (actors, director, writer, composer);
    }
}

class Program
{
    static async Task Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var api = new MovieApi("https://api.themoviedb.org/3/");
        Console.WriteLine("Завантаження найкращих фільмів...", Color.Yellow);

        var movies = await api.GetTopRatedMoviesAsync();
        if (movies != null && movies.Count > 0)
        {
            int count = 1;
            foreach (var movie in movies)
            {
                string title = movie.Title ?? "Без назви";
                string overview = movie.Overview ?? "Опис відсутній";
                string releaseDate = !string.IsNullOrEmpty(movie.ReleaseDate)
                    ? DateTime.Parse(movie.ReleaseDate).ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("uk-UA"))
                    : "Дата невідома";
                string voteCount = movie.VoteCount.HasValue ? $"{movie.VoteCount} голосів" : "Голоси відсутні";

                Console.Write($" {count}. ", Color.White);
                Console.WriteLine(title, Color.Yellow);
                Console.Write(" Дата випуску: ", Color.White);
                Console.WriteLine(releaseDate, Color.Green);
                Console.Write(" Рейтинг: ", Color.White);
                PrintRating(movie.VoteAverage, movie.VoteCount);

                var credits = await api.GetMovieCreditsAsync(movie.Id);
                List<string> actors = credits.Actors;
                string director = credits.Director;
                string writer = credits.Writer;
                string composer = credits.Composer;

                Console.WriteLine(" Актори:", Color.White);
                foreach (var actor in actors)
                {
                    Console.WriteLine($" - {actor}", Color.Magenta);
                }

                Console.Write(" Режисер: ", Color.White);
                Console.WriteLine(director, Color.Cyan);
                Console.Write(" Сценарист: ", Color.White);
                Console.WriteLine(writer, Color.Cyan);
                Console.Write(" Композитор: ", Color.White);
                Console.WriteLine(composer, Color.Cyan);

                Console.Write(" Опис:", Color.White);
                PrintMultilineText(overview, 40, Color.Cyan);

                Console.WriteLine(" " + new string('-', 50), Color.Gray);
                count++;
            }
        }
        else
        {
            Console.WriteLine("Не вдалося завантажити фільми.", Color.Red);
        }
    }

    static void PrintRating(double? voteAverage, int? voteCount)
    {
        if (!voteAverage.HasValue)
        {
            Console.WriteLine("Немає рейтингу", Color.Red);
            return;
        }

        double rating = voteAverage.Value;
        int fullStars = (int)Math.Round(rating);
        const int totalStars = 10;

        for (int i = 0; i < fullStars; i++)
        {
            Console.Write("*", Color.Yellow);
        }

        for (int i = fullStars; i < totalStars; i++)
        {
            Console.Write("*", Color.DarkGoldenrod);
        }

        Console.Write($" {rating} ({voteCount ?? 0} голосів)", Color.Red);
        Console.WriteLine();
    }

    static void PrintMultilineText(string text, int maxWidth, Color color)
    {
        string[] words = text.Split(' ');
        string line = " ";

        foreach (var word in words)
        {
            if (line.Length + word.Length + 1 > maxWidth)
            {
                Console.WriteLine(line, color);
                line = " " + word;
            }
            else
            {
                line += " " + word;
            }
        }

        if (!string.IsNullOrWhiteSpace(line))
            Console.WriteLine(line, color);
    }
}
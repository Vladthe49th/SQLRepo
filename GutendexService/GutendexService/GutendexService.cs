using RestSharp;
using Newtonsoft.Json.Linq;

namespace GutenbergConsoleApp.Services
{
    public class GutendexService
    {
        private readonly RestClient _client;

        public GutendexService()
        {
            _client = new RestClient("https://gutendex.com/");
        }

        // 1. Search of books by author
        public async Task<List<(int id, string title)>> SearchBooksByAuthorAsync(string authorName)
        {
            var request = new RestRequest($"books/?search={authorName}", Method.Get);
            var response = await _client.ExecuteAsync(request);

            if (!response.IsSuccessful || response.Content == null)
                throw new Exception("Error when getting data!");

            var json = JObject.Parse(response.Content);
            var results = json["results"];

            var books = new List<(int id, string title)>();
            foreach (var book in results!)
            {
                int id = (int)book["id"]!;
                string title = (string)book["title"]!;
                books.Add((id, title));
            }

            return books;
        }

        // 2. Getting book ID and details
        public async Task<JObject> GetBookDetailsAsync(int bookId)
        {
            var request = new RestRequest($"books/{bookId}/", Method.Get);
            var response = await _client.ExecuteAsync(request);

            if (!response.IsSuccessful || response.Content == null)
                throw new Exception("Couldn`t get the book data!");

            return JObject.Parse(response.Content);
        }
    }
}

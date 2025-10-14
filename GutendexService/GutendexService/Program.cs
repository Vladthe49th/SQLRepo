using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GutenbergConsoleApp.Services;
using Newtonsoft.Json.Linq;

namespace GutenbergConsoleApp
{
    internal class Program
    {
        static async Task Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = " Gutenberg Book Downloader";

            var service = new GutendexService();

            Console.WriteLine("=== Public library of Gutenberg ===");
            Console.Write("Enter a name of an author: ");
            string authorName = Console.ReadLine()?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(authorName))
            {
                Console.WriteLine("There must be a name!");
                return;
            }

            Console.WriteLine("\nSearching...");
            var books = await service.SearchBooksByAuthorAsync(authorName);

            if (books.Count == 0)
            {
                Console.WriteLine("No books found!");
                return;
            }

            Console.WriteLine($"\nFound {books.Count} books:");
            for (int i = 0; i < books.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {books[i].title} (ID: {books[i].id})");
            }

            Console.Write("\n Choose the number of a book: ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > books.Count)
            {
                Console.WriteLine("Wrong choice. Now perish!");
                return;
            }

            var selectedBook = books[choice - 1];
            Console.WriteLine($"\nYour book is: {selectedBook.title}");

            // Getting book details
            JObject details = await service.GetBookDetailsAsync(selectedBook.id);

            // Accessible formats
            var formats = details["formats"]
                ?.Children<JProperty>()
                .Where(p => p.Value.Type == JTokenType.String)
                .ToDictionary(p => p.Name, p => (string)p.Value!);

            if (formats == null || formats.Count == 0)
            {
                Console.WriteLine(" There are no formats for this book!");
                return;
            }

            Console.WriteLine("\nFormats this book can be downloaded in:");
            var formatList = formats.Keys.ToList();
            for (int i = 0; i < formatList.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {formatList[i]}");
            }

            Console.Write("\nChoose format number: ");
            if (!int.TryParse(Console.ReadLine(), out int formatChoice) ||
                formatChoice < 1 || formatChoice > formatList.Count)
            {
                Console.WriteLine("Wrong choice, now die!");
                return;
            }

            string chosenFormat = formatList[formatChoice - 1];
            string downloadUrl = formats[chosenFormat];

            Console.WriteLine($"\n⬇️ Downloading {chosenFormat}...");
            string fileName = $"{selectedBook.title}_{selectedBook.id}.{GetFileExtension(chosenFormat)}";
            string filePath = Path.Combine(Environment.CurrentDirectory, fileName);

            try
            {
                await DownloadFileAsync(downloadUrl, filePath);
                Console.WriteLine($"File saved: {filePath}");

                Console.WriteLine("\nPress enter to open your file!");
                Console.ReadLine();

                Process.Start(new ProcessStartInfo()
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Loading error: {ex.Message}");
            }
        }

        // File download method
        private static async Task DownloadFileAsync(string url, string filePath)
        {
            using var client = new System.Net.Http.HttpClient();
            var data = await client.GetByteArrayAsync(url);
            await File.WriteAllBytesAsync(filePath, data);
        }

        // File extension method
        private static string GetFileExtension(string formatKey)
        {
            if (formatKey.Contains("html")) return "html";
            if (formatKey.Contains("epub")) return "epub";
            if (formatKey.Contains("pdf")) return "pdf";
            if (formatKey.Contains("txt")) return "txt";
            return "dat";
        }
    }
}

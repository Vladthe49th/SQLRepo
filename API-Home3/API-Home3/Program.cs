using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Enter URL for a file that you want to download:");
        string url = Console.ReadLine();

        Console.WriteLine("Enter save path: ");
        string destinationPath = Console.ReadLine();

        // Downloading in a separate thread
        Task downloadTask = DownloadFileAsync(url, destinationPath);

        Console.WriteLine("Downloading has began! You can work with the program for now.");

        // User work emulation
        while (!downloadTask.IsCompleted)
        {
            Console.WriteLine("App is working! You can press ctrl + c to exit");
            Thread.Sleep(2000);
        }

        await downloadTask;

        Console.WriteLine("Loaading finished!");
    }

    static async Task DownloadFileAsync(string url, string destinationPath)
    {
        try
        {
            using HttpClient client = new HttpClient();
            using HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            using Stream contentStream = await response.Content.ReadAsStreamAsync(),
                          fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

            await contentStream.CopyToAsync(fileStream);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Download error: {ex.Message}");
        }
    }
}

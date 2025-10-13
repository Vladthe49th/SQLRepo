using System;
using RestSharp;

namespace GitHubInfo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Write("Enter github user nickname: ");
            string username = Console.ReadLine();

            // Making the client
            var client = new RestClient($"https://api.github.com/users/{username}");

            // User-Agent
            var request = new RestRequest();
            request.AddHeader("User-Agent", "RestSharpDemoApp");

            // Execute request
            var response = client.Execute<GitUser>(request);

            if (!response.IsSuccessful || response.Data == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Couldn`t get the data! Check if everything`s alright.");
                Console.ResetColor();
                return;
            }

            // Data
            var user = response.Data;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nUser data:");
            Console.ResetColor();

            Console.WriteLine($"Name: {user.Name ?? "немає"}");
            Console.WriteLine($"Login: {user.Login}");
            Console.WriteLine($"Avatar: {user.Avatar_url}");
            Console.WriteLine($"Registrtion data: {user.Created_at:dd.MM.yyyy}");
        }
    }
}


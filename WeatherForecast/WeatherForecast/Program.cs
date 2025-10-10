using System;
using RestSharp;
using Newtonsoft.Json.Linq;

namespace WeatherApp
{
    internal class Program
    {
        private const string API_KEY = "92d90c38e46c5a426c8f1b7b76e34cfc"; 

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = " WeatherApp";

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine("Welcome to our synoptic app!");
            Console.WriteLine("═══════════════════════════════════════════");
            Console.ResetColor();

            Console.Write(" Enter city name: ");
            string city = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(city))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("City not entered!");
                Console.ResetColor();
                return;
            }

            Console.WriteLine($"\n Getting data for {city}...\n");

            GetCurrentWeather(city);
            Console.WriteLine();
            PrintSeparator();

            GetTomorrowForecast(city);
            Console.WriteLine();
            PrintSeparator();

            GetSeaTemperature(API_KEY);

            Console.WriteLine("\nForecast complete!");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Press enter to exit...");
            Console.ResetColor();
            Console.ReadLine();
        }

        // ───────────────────────────────────────────────
        static void GetCurrentWeather(string city)
        {
            try
            {
                var client = new RestClient("https://api.openweathermap.org");
                var request = new RestRequest("data/2.5/weather", Method.Get);

                request.AddParameter("q", city);
                request.AddParameter("appid", API_KEY);
                request.AddParameter("units", "metric");
                request.AddParameter("lang", "ua");

                var response = client.Execute(request);

                if (!response.IsSuccessful)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Couldn`t get the weather: {response.StatusDescription}");
                    Console.ResetColor();
                    return;
                }

                var json = JObject.Parse(response.Content);
                string desc = json["weather"]?[0]?["description"]?.ToString() ?? "—";
                string icon = GetWeatherIcon(desc);

                double temp = json["main"]?["temp"]?.Value<double>() ?? double.NaN;
                double feels = json["main"]?["feels_like"]?.Value<double>() ?? double.NaN;
                int humidity = json["main"]?["humidity"]?.Value<int>() ?? 0;
                double wind = json["wind"]?["speed"]?.Value<double>() ?? 0;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Current weather:");
                Console.ResetColor();
                Console.WriteLine($"State: {desc} {icon}");
                Console.WriteLine($"Temperature: {temp}°C");
                Console.WriteLine($"Humidity: {humidity}%");
                Console.WriteLine($"Wind: {wind} m/s");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.Message}");
                Console.ResetColor();
            }
        }

        // ───────────────────────────────────────────────
        static void GetTomorrowForecast(string city)
        {
            try
            {
                var client = new RestClient("https://api.openweathermap.org");
                var request = new RestRequest("data/2.5/forecast", Method.Get);

                request.AddParameter("q", city);
                request.AddParameter("appid", API_KEY);
                request.AddParameter("units", "metric");
                request.AddParameter("lang", "ua");

                var response = client.Execute(request);

                if (!response.IsSuccessful)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Couldn`t get the forecast: {response.StatusDescription}");
                    Console.ResetColor();
                    return;
                }

                var json = JObject.Parse(response.Content);
                var list = json["list"] as JArray;

                if (list == null || list.Count < 9)
                {
                    Console.WriteLine("Tomorrow`s forecast is unaccessible!");
                    return;
                }

                var tomorrow = list[8];
                string desc = tomorrow["weather"]?[0]?["description"]?.ToString() ?? "—";
                string icon = GetWeatherIcon(desc);

                double temp = tomorrow["main"]?["temp"]?.Value<double>() ?? double.NaN;
                double feels = tomorrow["main"]?["feels_like"]?.Value<double>() ?? double.NaN;
                int humidity = tomorrow["main"]?["humidity"]?.Value<int>() ?? 0;
                double wind = tomorrow["wind"]?["speed"]?.Value<double>() ?? 0;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("FORECAST FOR TOMORROW:");
                Console.ResetColor();
                Console.WriteLine($"State: {desc} {icon}");
                Console.WriteLine($"Temperature: {temp}°C");
                Console.WriteLine($"💧 Вологість: {humidity}%");
                Console.WriteLine($"💨 Wind: {wind} m/s");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.Message}");
                Console.ResetColor();
            }
        }

        // ───────────────────────────────────────────────
        static void GetSeaTemperature(string apiKey)
        {
            try
            {
                var client = new RestClient("https://api.openweathermap.org");
                var request = new RestRequest("data/2.5/weather", Method.Get);

                // 📍 Координати Одеси
                request.AddParameter("lat", "46.4825");
                request.AddParameter("lon", "30.7233");
                request.AddParameter("appid", apiKey);
                request.AddParameter("units", "metric");

                var response = client.Execute(request);

                if (!response.IsSuccessful)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Couldn`t get the sea temperature: {response.StatusDescription}");
                    Console.ResetColor();
                    return;
                }

                var json = JObject.Parse(response.Content);
                double seaTemp = json["main"]?["temp"]?.Value<double>() ?? double.NaN;

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Black sea temperature (near Odessa):");
                Console.ResetColor();
                Console.WriteLine($"🌡 {seaTemp}°C");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.Message}");
                Console.ResetColor();
            }
        }

        // ───────────────────────────────────────────────
        static string GetWeatherIcon(string description)
        {
            if (string.IsNullOrEmpty(description)) return "☁️";
            description = description.ToLower();

            if (description.Contains("Sun") || description.Contains("clear")) return "☀️";
            if (description.Contains("rain") || description.Contains("злива")) return "🌧️";
            if (description.Contains("thunder")) return "⛈️";
            if (description.Contains("snow")) return "❄️";
            if (description.Contains("fog")) return "🌫️";
            if (description.Contains("cloud")) return "☁️";

            return "🌤️";
        }

        static void PrintSeparator()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("───────────────────────────────────────────────");
            Console.ResetColor();
        }
    }
}

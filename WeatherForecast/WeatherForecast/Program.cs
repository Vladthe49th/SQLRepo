using System;
using RestSharp;
using Newtonsoft.Json.Linq;

namespace WeatherApp
{
    internal class Program
    {
        private const string API_KEY = "YOUR_API_KEY"; // 🔑 заміни своїм ключем

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "🌤 Прогноз погоди - WeatherApp";

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine("🌍 Ласкаво просимо до нашого синоптика!");
            Console.WriteLine("═══════════════════════════════════════════");
            Console.ResetColor();

            Console.Write("🏙️  Введіть назву міста: ");
            string city = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(city))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("❌ Місто не введено.");
                Console.ResetColor();
                return;
            }

            Console.WriteLine($"\n📡 Отримуємо дані для міста {city}...\n");

            GetCurrentWeather(city);
            Console.WriteLine();
            PrintSeparator();

            GetTomorrowForecast(city);
            Console.WriteLine();
            PrintSeparator();

            GetSeaTemperature(API_KEY);

            Console.WriteLine("\n✅ Прогноз завершено!");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Натисніть Enter для виходу...");
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
                    Console.WriteLine($"❌ Не вдалося отримати погоду: {response.StatusDescription}");
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
                Console.WriteLine("⛅ ПОТОЧНА ПОГОДА:");
                Console.ResetColor();
                Console.WriteLine($"📖 Стан: {desc} {icon}");
                Console.WriteLine($"🌡 Температура: {temp}°C (відчувається як {feels}°C)");
                Console.WriteLine($"💧 Вологість: {humidity}%");
                Console.WriteLine($"💨 Вітер: {wind} м/с");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Помилка: {ex.Message}");
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
                    Console.WriteLine($"❌ Не вдалося отримати прогноз: {response.StatusDescription}");
                    Console.ResetColor();
                    return;
                }

                var json = JObject.Parse(response.Content);
                var list = json["list"] as JArray;

                if (list == null || list.Count < 9)
                {
                    Console.WriteLine("⚠️ Прогноз на завтра недоступний.");
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
                Console.WriteLine("📅 ПРОГНОЗ НА ЗАВТРА:");
                Console.ResetColor();
                Console.WriteLine($"📖 Стан: {desc} {icon}");
                Console.WriteLine($"🌡 Температура: {temp}°C (відчувається як {feels}°C)");
                Console.WriteLine($"💧 Вологість: {humidity}%");
                Console.WriteLine($"💨 Вітер: {wind} м/с");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Помилка: {ex.Message}");
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
                    Console.WriteLine($"❌ Не вдалося отримати температуру моря: {response.StatusDescription}");
                    Console.ResetColor();
                    return;
                }

                var json = JObject.Parse(response.Content);
                double seaTemp = json["main"]?["temp"]?.Value<double>() ?? double.NaN;

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("🌊 ТЕМПЕРАТУРА ВОДИ У ЧОРНОМУ МОРІ (біля Одеси):");
                Console.ResetColor();
                Console.WriteLine($"🌡 {seaTemp}°C");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Помилка: {ex.Message}");
                Console.ResetColor();
            }
        }

        // ───────────────────────────────────────────────
        static string GetWeatherIcon(string description)
        {
            if (string.IsNullOrEmpty(description)) return "☁️";
            description = description.ToLower();

            if (description.Contains("сон") || description.Contains("ясн")) return "☀️";
            if (description.Contains("дощ") || description.Contains("злива")) return "🌧️";
            if (description.Contains("гроза")) return "⛈️";
            if (description.Contains("сніг")) return "❄️";
            if (description.Contains("туман")) return "🌫️";
            if (description.Contains("хмар")) return "☁️";

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

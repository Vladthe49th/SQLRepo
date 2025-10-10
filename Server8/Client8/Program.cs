using System.Text;

class Program
{
    static async Task Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        string srcUrl = "https://www.gismeteo.ua/weather-odessa-4982/";

        string destFile = @"C:\1\weather.txt";

        try
        {
            using (var client = new HttpClient())
            {
                // завантажуємо вміст як потік
                using (var response = await client.GetAsync(srcUrl))
                {
                    Console.WriteLine($"Код відповіді: {(int)response.StatusCode} ({response.ReasonPhrase})");

                    response.EnsureSuccessStatusCode(); // перевіряємо успішність запиту (викликає виняток, якщо код не 2xx)
                    string content = await response.Content.ReadAsStringAsync();

                    // зберігаємо у файл
                    File.WriteAllText(destFile, content, Encoding.UTF8);
                    Console.WriteLine($"Збережено у {destFile}");
                    Console.WriteLine(content);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка: {ex.Message}");
        }
    }
}
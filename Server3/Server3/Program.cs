using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Collections.Concurrent;

class Server
{
    private const int DEFAULT_BUFLEN = 512;
    private const string DEFAULT_PORT = "27015";
    private static ConcurrentQueue<(Socket, byte[])> messageQueue = new ConcurrentQueue<(Socket, byte[])>();

    // Словник команд
    private static readonly Dictionary<string, Func<string, Task<string>>> commandHandlers =
        new Dictionary<string, Func<string, Task<string>>>(StringComparer.OrdinalIgnoreCase)
        {
            { "1", async (_) => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }, // дата + час
            { "2", async (_) => DateTime.Now.ToString("HH:mm:ss") },            // лише час
            { "3", GetWeatherAsync },  // прогноз погоди
            { "4", GetEuroRateAsync }, // курс євро
            { "5", GetBitcoinRateAsync } // курс біткоїна
        };

    static async Task Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.Title = "SERVER SIDE";
        Console.WriteLine("Процес сервера запущено!");

        try
        {
            var ipAddress = IPAddress.Any;
            var localEndPoint = new IPEndPoint(ipAddress, int.Parse(DEFAULT_PORT));

            var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(localEndPoint);
            listener.Listen(10);

            Console.WriteLine("Очікування клієнта...");

            var clientSocket = await listener.AcceptAsync();
            Console.WriteLine("Підключення встановлено успішно!");

            listener.Close();

            _ = ProcessMessages();

            while (true)
            {
                var buffer = new byte[DEFAULT_BUFLEN];
                int bytesReceived = await clientSocket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);

                if (bytesReceived > 0)
                {
                    messageQueue.Enqueue((clientSocket, buffer[..bytesReceived]));
                }
                else
                {
                    Console.WriteLine("Клієнт відключився.");
                    break;
                }
            }

            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
            Console.WriteLine("Сервер завершив роботу.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Сталася помилка: {ex.Message}");
        }
    }

    private static async Task ProcessMessages()
    {
        while (true)
        {
            if (messageQueue.TryDequeue(out var item))
            {
                var (clientSocket, buffer) = item;
                string message = Encoding.UTF8.GetString(buffer).Trim();

                Console.WriteLine($"Отримано команду від клієнта: {message}");

                string response;
                if (commandHandlers.TryGetValue(message.Split(' ')[0], out var handler))
                {
                    response = await handler(message);
                }
                else
                {
                    response = "Невідома команда. Використовуйте: 1 (дата+час), 2 (час), 3 <місто> (погода), 4 (євро), 5 (біткоїн)";
                }

                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                await clientSocket.SendAsync(new ArraySegment<byte>(responseBytes), SocketFlags.None);

                Console.WriteLine($"Надіслано клієнту: {response}");
            }

            await Task.Delay(50);
        }
    }

    // --- Команди ---
    private static async Task<string> GetWeatherAsync(string input)
    {
        
        var parts = input.Split(' ', 2);
        if (parts.Length < 2)
            return "Вкажіть місто. Наприклад: Одеса";

        string city = parts[1];
       
        await Task.Yield();
        return $"Прогноз погоди для {city}: +20°C, ясно";
    }

    private static async Task<string> GetEuroRateAsync(string _)
    {
        await Task.Yield();
        return "Курс EUR: 41.20 UAH";
    }

    private static async Task<string> GetBitcoinRateAsync(string _)
    {
        await Task.Yield();
        return "Курс BTC: 2 580 000 UAH";
    }
}



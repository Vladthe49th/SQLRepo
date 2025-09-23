using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server3
{
    public class NetworkServer
    {
        private const int DEFAULT_BUFLEN = 512;
        private readonly int port;
        private Socket listener;
        private Socket clientSocket;
        private readonly ConcurrentQueue<(Socket, byte[])> messageQueue = new();

        private readonly Dictionary<string, Func<string, Task<string>>> commandHandlers;

        public event Action<string> OnLog;        
        public event Action<string> OnMessage;    

        public NetworkServer(int port = 27015)
        {
            this.port = port;

       
            commandHandlers = new Dictionary<string, Func<string, Task<string>>>(StringComparer.OrdinalIgnoreCase)
        {
            { "1", async (_) => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },
            { "2", async (_) => DateTime.Now.ToString("HH:mm:ss") },
            { "3", GetWeatherAsync },
            { "4", GetEuroRateAsync },
            { "5", GetBitcoinRateAsync }
        };
        }

        public async Task StartAsync()
        {
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(new IPEndPoint(IPAddress.Any, port));
            listener.Listen(10);

            OnLog?.Invoke("Сервер запущено. Очікування клієнта...");
            clientSocket = await listener.AcceptAsync();
            OnLog?.Invoke("Клієнт підключився!");

            _ = Task.Run(ProcessMessages);

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
                    OnLog?.Invoke("Клієнт відключився.");
                    break;
                }
            }
        }

        private async Task ProcessMessages()
        {
            while (true)
            {
                if (messageQueue.TryDequeue(out var item))
                {
                    var (socket, buffer) = item;
                    string message = Encoding.UTF8.GetString(buffer).Trim();

                    OnMessage?.Invoke($"Команда від клієнта: {message}");

                    string response;
                    if (commandHandlers.TryGetValue(message.Split(' ')[0], out var handler))
                    {
                        response = await handler(message);
                    }
                    else
                    {
                        response = "Невідома команда. Використовуйте: 1, 2, 3 <місто>, 4, 5";
                    }

                    byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                    await socket.SendAsync(new ArraySegment<byte>(responseBytes), SocketFlags.None);

                    OnMessage?.Invoke($"Відповідь: {response}");
                }

                await Task.Delay(50);
            }
        }

        // --- Команды ---
        private static async Task<string> GetWeatherAsync(string input)
        {
            var parts = input.Split(' ', 2);
            if (parts.Length < 2)
                return "Вкажіть місто. Наприклад: 3 Київ";

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

}

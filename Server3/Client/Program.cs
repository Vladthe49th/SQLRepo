using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Diagnostics;
using Client;

namespace Client
{
    class Client
    {
        private const int DEFAULT_BUFLEN = 512;
        private const string DEFAULT_PORT = "27015";

        static async Task Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "CLIENT SIDE";

            try
            {
                var ipAddress = IPAddress.Loopback;
                var remoteEndPoint = new IPEndPoint(ipAddress, int.Parse(DEFAULT_PORT));

                var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                //  Перевірка
                bool serverUp = await IsServerAvailable(remoteEndPoint);
                if (!serverUp)
                {
                    Console.WriteLine("Сервер не знайдено. Запускаємо його...");
                    try
                    {
                        Process.Start("Server.exe"); 
                        await Task.Delay(2000); 
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Не вдалося запустити сервер: {ex.Message}");
                        return;
                    }
                }

                await clientSocket.ConnectAsync(remoteEndPoint);
                Console.WriteLine("Підключення до сервера встановлено.");

                // Надсилання
                var sendingTask = Task.Run(async () =>
                {
                    while (true)
                    {
                        Console.WriteLine("\nМеню:");
                        Console.WriteLine("1 - отримати дату і час");
                        Console.WriteLine("2 - отримати лише час");
                        Console.WriteLine("3 <місто> - отримати прогноз погоди");
                        Console.WriteLine("4 - отримати курс євро");
                        Console.WriteLine("5 - отримати курс біткоїна");
                        Console.WriteLine("exit - вихід");
                        Console.Write("Ваш вибір: ");

                        var message = Console.ReadLine();

                        if (string.IsNullOrEmpty(message)) continue;
                        if (message.ToLower() == "exit") break;

                        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                        await clientSocket.SendAsync(new ArraySegment<byte>(messageBytes), SocketFlags.None);
                        Console.WriteLine($"Запит надіслано: {message}");
                    }

                    clientSocket.Shutdown(SocketShutdown.Send);
                });

                //  Oтримання
                var receivingTask = Task.Run(async () =>
                {
                    while (true)
                    {
                        var buffer = new byte[DEFAULT_BUFLEN];
                        int bytesReceived = await clientSocket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                        if (bytesReceived > 0)
                        {
                            string response = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                            Console.WriteLine($"\nВідповідь від сервера: {response}\n");
                        }
                        else
                        {
                            break; 
                        }
                    }
                });

                await Task.WhenAll(sendingTask, receivingTask);

                clientSocket.Close();
                Console.WriteLine("З’єднання з сервером закрито.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Сталася помилка: {ex.Message}");
            }
        }

        //  Перевірка доступності
        private static async Task<bool> IsServerAvailable(IPEndPoint endpoint)
        {
            try
            {
                using (var testSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    var connectTask = testSocket.ConnectAsync(endpoint);
                    var timeoutTask = Task.Delay(1000);

                    var completed = await Task.WhenAny(connectTask, timeoutTask);
                    if (completed == timeoutTask)
                        return false;

                    return testSocket.Connected;
                }
            }
            catch
            {
                return false;
            }
        }
    }

}


class Program
{
    static async Task Main()
    {
        var client = new NetworkClient();
        client.OnLog += msg => Console.WriteLine($"[LOG] {msg}");
        client.OnMessage += msg => Console.WriteLine($"[SERVER] {msg}");

        await client.ConnectAsync();

        while (true)
        {
            Console.WriteLine("Меню: 1 (дата+час), 2 (час), 3 <місто> (погода), 4 (євро), 5 (біткоїн), exit");
            string input = Console.ReadLine();
            if (input?.ToLower() == "exit") break;

            await client.SendMessageAsync(input);
        }
    }
}

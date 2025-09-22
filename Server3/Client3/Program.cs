using System.Net;
using System.Net.Sockets;
using System.Text;

class Client
{
    private const int DEFAULT_BUFLEN = 512;
    private const string DEFAULT_PORT = "27015";

    static async Task Main()
    {
        Console.Title = "CLIENT SIDE";
        try
        {
            var ipAddress = IPAddress.Loopback;
            var remoteEndPoint = new IPEndPoint(ipAddress, int.Parse(DEFAULT_PORT));

            var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            await clientSocket.ConnectAsync(remoteEndPoint);
            Console.WriteLine("Підключення до сервера встановлено.");

            // завдання надсилання повідомлень
            var sendingTask = Task.Run(async () =>
            {
                while (true)
                {
                    Console.WriteLine("\nМеню:");
                    Console.WriteLine("1 - отримати дату і час");
                    Console.WriteLine("2 - отримати лише час");
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


            // завдання отримання повідомлень
            var receivingTask = Task.Run(async () =>
            {
                while (true)
                {
                    var buffer = new byte[DEFAULT_BUFLEN];
                    int bytesReceived = await clientSocket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                    if (bytesReceived > 0)
                    {
                        string response = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                        Console.Clear();
                        Console.Title = $"Відповідь від сервера: {response}";
                        Console.WriteLine($"Відповідь від сервера: {response}");
                    }
                    else
                    {
                        // якщо сервер закрив з’єднання, вийти з циклу
                        break;
                    }
                }
            });

            // очікування завершення обох завдань
            await Task.WhenAll(sendingTask, receivingTask);

            clientSocket.Close();
            Console.WriteLine("З’єднання з сервером закрито.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Сталася помилка: {ex.Message}");
        }
    }
}
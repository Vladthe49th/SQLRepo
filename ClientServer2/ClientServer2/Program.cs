using System.Net;
using System.Net.Sockets;
using System.Text;

class Client
{
    private const int DEFAULT_BUFLEN = 512;
    private const string DEFAULT_PORT = "27015";

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.Title = "CLIENT SIDE";

        try
        {
            var ipAddress = IPAddress.Loopback;
            var remoteEndPoint = new IPEndPoint(ipAddress, int.Parse(DEFAULT_PORT));

            var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(remoteEndPoint);

            Console.WriteLine("Подключение к серверу установлено.");
            Console.WriteLine("\nДоступные команды:");
            Console.WriteLine("1) привет");
            Console.WriteLine("2) как дела");
            Console.WriteLine("3) время");
            Console.WriteLine("4) любое целое число (сервер вернёт число+1)");
            Console.WriteLine("5) exit (для выхода)\n");

            while (true)
            {
                Console.Write("Введите сообщение: ");
                string input = Console.ReadLine() ?? "";

                byte[] messageBytes = Encoding.UTF8.GetBytes(input);
                clientSocket.Send(messageBytes);

                var buffer = new byte[DEFAULT_BUFLEN];
                int bytesReceived = clientSocket.Receive(buffer);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                Console.WriteLine($"Ответ от сервера: {response}");

                if (input.ToLower() == "exit")
                    break;
            }

            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
            Console.WriteLine("Соединение закрыто.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
}

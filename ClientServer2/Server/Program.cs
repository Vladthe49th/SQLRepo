using System.Net;
using System.Net.Sockets;
using System.Text;

class Server
{
    private const int DEFAULT_BUFLEN = 512;
    private const string DEFAULT_PORT = "27015";

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.Title = "SERVER SIDE";
        Console.WriteLine("Сервер запущен...");

        try
        {
            var ipAddress = IPAddress.Any;
            var localEndPoint = new IPEndPoint(ipAddress, int.Parse(DEFAULT_PORT));

            var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(localEndPoint);
            listener.Listen(10);

            Console.WriteLine("Ожидание подключения клиента...");
            var clientSocket = listener.Accept();
            Console.WriteLine("Клиент подключился!");

            while (true)
            {
                var buffer = new byte[DEFAULT_BUFLEN];
                int bytesReceived = clientSocket.Receive(buffer);

                if (bytesReceived > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                    Console.WriteLine($"Сообщение от клиента: {message}");

                    // Проверяем команды
                    string response;
                    if (message.ToLower() == "как дела")
                        response = "Лучше всех";
                    else if (message.ToLower() == "привет")
                        response = "И тебе привет!";
                    else if (message.ToLower() == "время")
                        response = $"Сейчас: {DateTime.Now}";
                    else if (message.ToLower() == "exit")
                    {
                        response = "Соединение закрывается...";
                        clientSocket.Send(Encoding.UTF8.GetBytes(response));
                        break;
                    }
                    else if (int.TryParse(message, out int number))
                        response = (number + 1).ToString();
                    else
                        response = "Неизвестная команда";

                    byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                    clientSocket.Send(responseBytes);
                    Console.WriteLine($"Ответ серверa: {response}");
                }
            }

            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
            Console.WriteLine("Сервер завершил работу.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
}


using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Collections.Concurrent;

class Server
{
    private const int DEFAULT_BUFLEN = 512;
    private const string DEFAULT_PORT = "27015";
    private static ConcurrentQueue<(Socket, byte[])> messageQueue = new ConcurrentQueue<(Socket, byte[])>();

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
            listener.Bind(localEndPoint); // await Task.Run(() => listener.Bind(localEndPoint));
            listener.Listen(10); // не блокує потік у тому сенсі, що не вимагає тривалих очікувань, тому для нього не передбачено асинхронну версію
            Console.WriteLine("Починається прослуховування інформації від клієнта.\nБудь ласка, запустіть клієнтську програму!");

            var clientSocket = await listener.AcceptAsync();
            Console.WriteLine("Підключення з клієнтською програмою встановлено успішно!");

            listener.Close();

            _ = ProcessMessages(); // var processMessagesTask = ProcessMessages();
            // знак підкреслення _ в C# називається discard (або "відкидання")
            // результат асинхронної операції не використовується, тому зберігати посилання на Task немає сенсу

            while (true)
            {
                var buffer = new byte[DEFAULT_BUFLEN];
                int bytesReceived = await clientSocket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None); // можуть бути вказані флаги OutOfBandInline, Peek, DontRoute, MaxIOVectorLength, Truncated
                // але всі ці флаги досить специфічні, деталі за посиланням https://learn.microsoft.com/uk-ua/dotnet/api/system.net.sockets.socketflags?view=net-9.0

                if (bytesReceived > 0)
                {

                    messageQueue.Enqueue((clientSocket, buffer));
                    Console.WriteLine($"Додано повідомлення до черги.");
                }
                else
                {
                    Console.WriteLine("Помилка при отриманні даних.");
                    break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                    Console.WriteLine($"Сообщение от клиента: {message}");

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

            clientSocket.Shutdown(SocketShutdown.Send);
            clientSocket.Close();
            Console.WriteLine("Процес сервера завершує свою роботу!");
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
                string message = Encoding.UTF8.GetString(buffer).Trim('\0', '\r', '\n', ' ');
                Console.WriteLine($" Клієнт-хозяін надіслав повідомлення!: {message}");

                await Task.Delay(200);

                string response;
                if (message == "1")
                {
                    response = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                }
                else if (message == "2")
                {
                    response = DateTime.Now.ToString("HH:mm:ss.fff");
                }
                else
                {
                    response = "Невірна команда. Використайте 1 (дата) або 2 (час).";
                }

                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                await clientSocket.SendAsync(new ArraySegment<byte>(responseBytes), SocketFlags.None);
                Console.WriteLine($"Процес сервера надсилає відповідь: {response}");
            }

            await Task.Delay(100);
        }
    }

}
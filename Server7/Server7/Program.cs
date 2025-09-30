using System.Net;
using System.Net.Sockets;
using System.Text;

class TcpChatServer
{
    private readonly int port;
    private readonly List<TcpClient> clients = new();
    private readonly StringBuilder messageHistory = new();

    public TcpChatServer(int port)
    {
        this.port = port;
    }

    public async Task StartAsync()
    {
        var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Console.WriteLine($"Сервер запущено на порту {port}");

        while (true)
        {
            var client = await listener.AcceptTcpClientAsync();
            lock (clients) clients.Add(client);

            Console.WriteLine("Новий клієнт підключився!");
            _ = HandleClientAsync(client);
        }
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        var stream = client.GetStream();

        // Надсилаємо історію новому клієнту
        var history = Encoding.UTF8.GetBytes(messageHistory.ToString());
        await stream.WriteAsync(history, 0, history.Length);

        var buffer = new byte[1024];
        while (true)
        {
            int byteCount;
            try
            {
                byteCount = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (byteCount == 0) break; // клієнт відключився
            }
            catch
            {
                break;
            }

            var message = Encoding.UTF8.GetString(buffer, 0, byteCount);
            var formattedMessage = $"[{DateTime.Now:T}] {message}";

            Console.WriteLine(formattedMessage);
            messageHistory.AppendLine(formattedMessage);
            await BroadcastAsync(formattedMessage, client);
        }

        lock (clients) clients.Remove(client);
        client.Close();
    }

    private async Task BroadcastAsync(string message, TcpClient sender)
    {
        var data = Encoding.UTF8.GetBytes(message);

        lock (clients)
        {
            foreach (var c in clients)
            {
                if (c == sender) continue;
                try
                {
                    c.GetStream().WriteAsync(data, 0, data.Length);
                }
                catch { }
            }
        }
    }
}

class Program
{
    static async Task Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        var server = new TcpChatServer(9000);
        await server.StartAsync();
    }
}

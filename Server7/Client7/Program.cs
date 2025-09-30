using System.Net.Sockets;
using System.Text;

class TcpChatClient
{
    private readonly string host;
    private readonly int port;
    private TcpClient? client;

    public TcpChatClient(string host, int port)
    {
        this.host = host;
        this.port = port;
    }

    public async Task ConnectAsync()
    {
        client = new TcpClient();
        await client.ConnectAsync(host, port);
        Console.WriteLine("Підключено до сервера!");

        _ = Task.Run(ReceiveMessagesAsync);
        await SendMessagesAsync();
    }

    private async Task ReceiveMessagesAsync()
    {
        var buffer = new byte[1024];
        var stream = client!.GetStream();

        while (true)
        {
            int byteCount = await stream.ReadAsync(buffer, 0, buffer.Length);
            if (byteCount == 0) break;

            var message = Encoding.UTF8.GetString(buffer, 0, byteCount);
            Console.WriteLine(message);
        }
    }

    private async Task SendMessagesAsync()
    {
        var stream = client!.GetStream();

        while (true)
        {
            var message = Console.ReadLine();
            if (message == "off") break;

            var data = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(data, 0, data.Length);
        }

        client.Close();
        Console.WriteLine("Відключено від сервера.");
    }
}

class Program
{
    static async Task Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        var client = new TcpChatClient("https://render.com", 9000);
        await client.ConnectAsync();
    }
}

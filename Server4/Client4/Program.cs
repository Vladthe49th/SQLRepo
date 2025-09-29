using System.Net.Sockets;
using System.Net;
using System.Text.Json;
using System.Text;

class ChatMessage
{
    public string Type { get; set; } = "Message";
    public string Name { get; set; } = "";
    public string Color { get; set; } = "White";
    public string Text { get; set; } = "";
    public DateTime Time { get; set; } = DateTime.Now;
}

class UdpChatClient
{
    private const int serverPort = 9000;
    private UdpClient? client;
    private IPEndPoint? serverEndpoint;
    private string userName = "";
    private ConsoleColor userColor = ConsoleColor.White;

    public async Task StartAsync()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.Title = "КЛІЄНТСЬКА СТОРОНА";

        Console.Write("Введіть нік: ");
        userName = Console.ReadLine() ?? "Anonymous";

        Console.Write("Введіть колір (Red, Green, Blue...): ");
        if (Enum.TryParse(Console.ReadLine(), true, out ConsoleColor color))
            userColor = color;

        var serverIp = "127.0.0.1";
        serverEndpoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);

        client = new UdpClient(0);
        client.Connect(serverEndpoint);

     
        await SendAsync(new ChatMessage
        {
            Type = "Join",
            Name = userName,
            Color = userColor.ToString(),
            Time = DateTime.Now
        });

        _ = Task.Run(ReceiveMessagesAsync);
        await SendMessagesAsync();
    }

    private async Task ReceiveMessagesAsync()
    {
        while (true)
        {
            var result = await client.ReceiveAsync();
            var json = Encoding.UTF8.GetString(result.Buffer);
            var msg = JsonSerializer.Deserialize<ChatMessage>(json);

            if (msg == null) continue;

            if (msg.Type == "System")
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine($"[{msg.Time:HH:mm}] {msg.Text}");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = Enum.TryParse(msg.Color, true, out ConsoleColor col) ? col : ConsoleColor.White;
                Console.WriteLine($"[{msg.Time:HH:mm}] {msg.Name}: {msg.Text}");
                Console.ResetColor();
            }
        }
    }

    private async Task SendMessagesAsync()
    {
        while (true)
        {
            var text = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(text))
                continue;

            if (text == "off")
            {
                await SendAsync(new ChatMessage
                {
                    Type = "Leave",
                    Name = userName,
                    Time = DateTime.Now
                });
                break;
            }

            await SendAsync(new ChatMessage
            {
                Type = "Message",
                Name = userName,
                Color = userColor.ToString(),
                Text = text,
                Time = DateTime.Now
            });
        }

        client.Close();
        Console.WriteLine("Відключено від сервера.");
    }

    private async Task SendAsync(ChatMessage msg)
    {
        var json = JsonSerializer.Serialize(msg);
        var data = Encoding.UTF8.GetBytes(json);
        await client.SendAsync(data, data.Length);
    }

    static async Task Main() => await new UdpChatClient().StartAsync();

}

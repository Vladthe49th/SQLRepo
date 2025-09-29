using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

class ChatMessage
{
    public string Type { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
    public string Text { get; set; }
    public DateTime Time { get; set; } = DateTime.Now;
}

class UdpChatServer
{
    private const int port = 9000;
    private UdpClient? server;
    private ConcurrentDictionary<IPEndPoint, string> clients = new(); 

    public async Task StartAsync()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.Title = "СЕРВЕРНА СТОРОНА";

        server = new UdpClient(port);
        Console.WriteLine($"Сервер запущено на порту {port}.");

        await ReceiveMessagesAsync();
    }

    private async Task ReceiveMessagesAsync()
    {
        while (true)
        {
            var result = await server.ReceiveAsync();
            var json = Encoding.UTF8.GetString(result.Buffer);

            var msg = JsonSerializer.Deserialize<ChatMessage>(json);
            if (msg == null) continue;

            if (msg.Type == "Join")
            {
                clients[result.RemoteEndPoint] = msg.Name;
                Console.WriteLine($"[{msg.Time:HH:mm}] {msg.Name} приєднався до чату");

                var sysMsg = new ChatMessage
                {
                    Type = "System",
                    Text = $"{msg.Name} приєднався до чату",
                    Time = DateTime.Now
                };
                await BroadcastAsync(sysMsg, result.RemoteEndPoint);
            }
            else if (msg.Type == "Leave")
            {
                clients.TryRemove(result.RemoteEndPoint, out var name);
                Console.WriteLine($"[{msg.Time:HH:mm}] {name} покинув чат");

                var sysMsg = new ChatMessage
                {
                    Type = "System",
                    Text = $"{name} покинув чат",
                    Time = DateTime.Now
                };
                await BroadcastAsync(sysMsg, result.RemoteEndPoint);
            }
            else
            {
                
                await BroadcastRawAsync(json, result.RemoteEndPoint);
            }
        }
    }

    private async Task BroadcastAsync(ChatMessage msg, IPEndPoint exclude)
    {
        var json = JsonSerializer.Serialize(msg);
        await BroadcastRawAsync(json, exclude);
    }

    private async Task BroadcastRawAsync(string json, IPEndPoint exclude)
    {
        var data = Encoding.UTF8.GetBytes(json);
        foreach (var client in clients.Keys)
        {
            if (!client.Equals(exclude))
                await server.SendAsync(data, data.Length, client);
        }
    }

    static async Task Main() => await new UdpChatServer().StartAsync();
}

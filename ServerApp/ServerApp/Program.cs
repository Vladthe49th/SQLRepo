using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
    static Dictionary<IPEndPoint, List<string>> clientSubscriptions = new();
    static List<IPEndPoint> removedClients = new();

    static void Main()
    {
        const int serverPort = 5000;
        const string multicastGroup = "239.0.0.222";

        using UdpClient udpServer = new UdpClient();
        udpServer.EnableBroadcast = true;

        Console.Title = "=== SERVER APP ===";
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("=== SERVER STARTED ===");
        Console.ResetColor();
        Console.WriteLine("Enter message or command:");

        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("> ");
            Console.ResetColor();
            string? input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                continue;

            if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                break;

            if (input.Equals("help", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Commands:");
                Console.WriteLine("  list - show all the clients");
                Console.WriteLine("  remove <ip> - delete a client");
                Console.WriteLine("  exit - finish the server work");
                continue;
            }

            if (input.Equals("list", StringComparison.OrdinalIgnoreCase))
            {
                if (clientSubscriptions.Count == 0)
                {
                    Console.WriteLine("There are no clients for now.");
                }
                else
                {
                    Console.WriteLine("Client list:");
                    foreach (var kvp in clientSubscriptions)
                    {
                        string ip = kvp.Key.Address.ToString();
                        string cats = string.Join(", ", kvp.Value);
                        bool removed = removedClients.Contains(kvp.Key);
                        Console.WriteLine($" - {ip} : [{cats}] {(removed ? "(DELETED)" : "")}");
                    }
                }
                continue;
            }

            if (input.StartsWith("remove ", StringComparison.OrdinalIgnoreCase))
            {
                string ip = input.Substring(7).Trim();
                var toRemove = clientSubscriptions.Keys
                    .FirstOrDefault(k => k.Address.ToString() == ip);

                if (toRemove != null)
                {
                    removedClients.Add(toRemove);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Client {ip} has been deleted.");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"Client {ip} not found!");
                }
                continue;
            }

            SendMessageToClients(udpServer, input, multicastGroup, serverPort);
        }

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Server has finished working.");
        Console.ResetColor();
    }

    static void SendMessageToClients(UdpClient udpServer, string message, string multicastGroup, int port)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);

        foreach (var kvp in clientSubscriptions)
        {
            if (removedClients.Contains(kvp.Key))
                continue; 

            udpServer.Send(data, data.Length, kvp.Key);
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[SENT]: {message}");
        Console.ResetColor();
    }
}

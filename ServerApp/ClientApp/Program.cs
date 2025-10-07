using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp
{
    internal class Program
    {

        [STAThread]
        static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());

            Console.Title = "ClientApp - Multicast & Broadcast Receiver";
            Console.WriteLine("=== CLIENT STARTED ===");

            // Client subscriptions
            Console.WriteLine("Choose categories to subscribe to:");
            Console.WriteLine("1 - News, 2 - Ads | 3 - Technical");
            Console.Write("Your choice: ");
            string? input = Console.ReadLine();

            var selected = input?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? Array.Empty<string>();

            var multicastGroups = new (string address, string name)[]
            {
                ("239.0.0.1", "News"),
                ("239.0.0.2", "Ads"),
                ("239.0.0.3", "Technical")
            };

            // UDP client for multicast
            var udpMulticast = new UdpClient(5000);
            foreach (var group in multicastGroups)
                udpMulticast.JoinMulticastGroup(IPAddress.Parse(group.address));

            // UDP client for broadcast
            var udpBroadcast = new UdpClient(5001);
            udpBroadcast.EnableBroadcast = true;
            IPEndPoint broadcastEP = new IPEndPoint(IPAddress.Any, 5001);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nClient subscriptions:");
            foreach (var c in selected)
            {
                if (int.TryParse(c, out int index) && index >= 1 && index <= 3)
                    Console.WriteLine($"- {multicastGroups[index - 1].name}");
            }
            Console.ResetColor();
            Console.WriteLine("\nWaiting for notifications...\n");

            // Multicast & broadcast
            Task.Run(() => ReceiveMulticast(udpMulticast, multicastGroups, selected));
            Task.Run(() => ReceiveBroadcast(udpBroadcast, broadcastEP));

            Console.ReadLine(); 
        }

        static void ReceiveMulticast(UdpClient client, (string address, string name)[] groups, string[] selected)
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            while (true)
            {
                byte[] data = client.Receive(ref remoteEP);
                string msg = Encoding.UTF8.GetString(data);

                bool show = false;
                foreach (var c in selected)
                {
                    if (int.TryParse(c, out int index) && msg.Contains(groups[index - 1].name))
                        show = true;
                }

                if (show)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(msg);
                    Console.ResetColor();
                }
            }
        }

        static void ReceiveBroadcast(UdpClient client, IPEndPoint ep)
        {
            while (true)
            {
                byte[] data = client.Receive(ref ep);
                string msg = Encoding.UTF8.GetString(data);

                if (msg.Contains("EMERGENCY"))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(msg);
                    Console.ResetColor();
                }
            }
        }
    }
}


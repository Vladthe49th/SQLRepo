using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class NetworkClient
    {
        private const int DEFAULT_BUFLEN = 512;
        private readonly int port;
        private readonly string host;
        private Socket clientSocket;

        public event Action<string> OnLog;
        public event Action<string> OnMessage;

        public NetworkClient(string host = "127.0.0.1", int port = 27015)
        {
            this.host = host;
            this.port = port;
        }

        public async Task ConnectAsync()
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            await clientSocket.ConnectAsync(new IPEndPoint(IPAddress.Parse(host), port));
            OnLog?.Invoke("Підключення до сервера встановлено.");

            _ = Task.Run(ReceiveMessagesAsync);
        }

        public async Task SendMessageAsync(string message)
        {
            if (clientSocket == null || !clientSocket.Connected) return;

            byte[] bytes = Encoding.UTF8.GetBytes(message);
            await clientSocket.SendAsync(new ArraySegment<byte>(bytes), SocketFlags.None);
            OnLog?.Invoke($"Надіслано: {message}");
        }

        private async Task ReceiveMessagesAsync()
        {
            while (true)
            {
                var buffer = new byte[DEFAULT_BUFLEN];
                int bytesReceived = await clientSocket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);

                if (bytesReceived > 0)
                {
                    string response = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                    OnMessage?.Invoke(response);
                }
                else
                {
                    OnLog?.Invoke("З'єднання закрито сервером.");
                    break;
                }
            }
        }
    }

}

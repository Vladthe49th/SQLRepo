using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using Shared;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.CursorVisible = false;
            Console.Clear();

            try
            {
                using var client = new TcpClient("127.0.0.1", 5000);
                NetworkStream ns = client.GetStream();

                // Thread for accepting GameState JSON
                Thread receiveThread = new Thread(() =>
                {
                    while (client.Connected)
                    {
                        string json = ReadMessage(ns);
                        if (json == null) break;

                        try
                        {
                            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                            var state = JsonSerializer.Deserialize<GameState>(json, options);
                            if (state == null) continue;

                            // HUD & Map rendering
                            RenderState(state);

                            if (state.GameOver)
                            {
                                
                                Console.SetCursorPosition(0, Console.CursorTop + 2);
                                Console.WriteLine(state.ResultText);
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.SetCursorPosition(0, 0);
                            Console.WriteLine("Bad GameState JSON: " + ex.Message);
                        }
                    }

                    Console.SetCursorPosition(0, Console.WindowHeight - 2);
                    Console.WriteLine("Disconnected from server. Press any key...");
                })
                { IsBackground = true };
                receiveThread.Start();

                // Main threat - movement commands
                while (client.Connected)
                {
                    var keyInfo = Console.ReadKey(true);
                    string direction = keyInfo.Key switch
                    {
                        ConsoleKey.UpArrow => "up",
                        ConsoleKey.DownArrow => "down",
                        ConsoleKey.LeftArrow => "left",
                        ConsoleKey.RightArrow => "right",
                        ConsoleKey.W => "up",
                        ConsoleKey.S => "down",
                        ConsoleKey.A => "left",
                        ConsoleKey.D => "right",
                        ConsoleKey.Escape => null,
                        _ => ""
                    };

                    if (direction == null)
                    {
                        break; // exit
                    }

                    if (!string.IsNullOrEmpty(direction))
                    {
                        var cmd = new Command { Type = CommandType.Move, Data = direction };
                        string jsonCmd = JsonSerializer.Serialize(cmd);
                        SendMessage(ns, jsonCmd);
                    }
                }

                try { client.Close(); } catch { }
                Console.CursorVisible = true;
            }
            catch (Exception ex)
            {
                Console.CursorVisible = true;
                Console.WriteLine("Client error: " + ex.Message);
            }
        }

        static void RenderState(GameState state)
        {
            try
            {
                // HUD height 
                int hudLines = Math.Max(3, 3 + state.Players.Count);
                Console.SetCursorPosition(0, 0);

                // Writing HUD
                Console.WriteLine($"⏱ Time left: {state.TimeLeft}s".PadRight(80));
                Console.WriteLine("Scores: " + string.Join("  ", state.Players.ConvertAll(p => $"{p.Symbol}:{p.Score}")).PadRight(80));
                foreach (var p in state.Players)
                {
                    Console.WriteLine($"{p.Symbol} at ({p.X},{p.Y})".PadRight(80));
                }
                Console.WriteLine(new string('-', 80));

              
                Console.Write(state.MapText);
            }
            catch
            {
               
                Console.Clear();
                Console.WriteLine(state.MapText);
            }
        }

        // Framing
        private static bool ReadExactly(NetworkStream ns, byte[] buffer, int offset, int size)
        {
            int read = 0;
            while (read < size)
            {
                int r;
                try { r = ns.Read(buffer, offset + read, size - read); }
                catch { return false; }
                if (r == 0) return false;
                read += r;
            }
            return true;
        }

        private static string ReadMessage(NetworkStream ns)
        {
            var lenBuf = new byte[4];
            if (!ReadExactly(ns, lenBuf, 0, 4)) return null;
            int len = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(lenBuf, 0));
            if (len <= 0 || len > 200_000) return null;
            var payload = new byte[len];
            if (!ReadExactly(ns, payload, 0, len)) return null;
            return Encoding.UTF8.GetString(payload);
        }

        private static void SendMessage(NetworkStream ns, string message)
        {
            try
            {
                var payload = Encoding.UTF8.GetBytes(message);
                var len = IPAddress.HostToNetworkOrder(payload.Length);
                var lenBuf = BitConverter.GetBytes(len);
                ns.Write(lenBuf, 0, lenBuf.Length);
                ns.Write(payload, 0, payload.Length);
                ns.Flush();
            }
            catch
            {
                // ignore
            }
        }
    }
}

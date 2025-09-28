
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;
using Shared;
using System.Linq;
using System.IO;

class Program
{
    private static TcpListener listener;
    private static TcpClient client;
    private static NetworkStream stream;
    private static char[,] map;
    private static Player clientPlayer;
    private static Player serverPlayer;
    private static int width = 20;
    private static int height = 10;
    private static DateTime endTime;
    private static System.Timers.Timer timer;
    private static Player[] players;

    static void Main()
    {
        InitializeMap();

        clientPlayer = new Player { Symbol = "B", X = 1, Y = 1 }; // Blue
        serverPlayer = new Player { Symbol = "R", X = width - 2, Y = 1 }; // Red
        players = new[] { clientPlayer, serverPlayer };

        map[clientPlayer.Y, clientPlayer.X] = clientPlayer.Symbol[0];
        map[serverPlayer.Y, serverPlayer.X] = serverPlayer.Symbol[0];

        listener = new TcpListener(IPAddress.Any, 5000);
        listener.Start();
        Console.WriteLine("Server started. Waiting for connection...");

        client = listener.AcceptTcpClient();
        Console.WriteLine("Client connected!");
        stream = client.GetStream();

        // Sending map to client
        SendMap();

        // Starting client
        endTime = DateTime.Now.AddMinutes(1);
        timer = new System.Timers.Timer(1000);
        timer.Elapsed += UpdateTimer;
        timer.Start();

        Thread receiveThread = new Thread(ReceiveData);
        receiveThread.Start();

        while (true)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                HandleMove(key, serverPlayer);
                SendMap();
            }
        }
    }

    private static void InitializeMap()
    {
        map = new char[height, width];

        // Filling the walls
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                map[y, x] = '#';

        // Making a labyrinth
        for (int y = 1; y < height - 1; y++)
            for (int x = 1; x < width - 1; x++)
                map[y, x] = '.';

        // Adding chests
        PlaceChests(8);

        // Finish
        map[height - 2, width - 2] = 'F';

      
    }

    private static void PlaceChests(int count)
    {
        var rand = new Random();
        int placed = 0;

        while (placed < count)
        {
            int x = rand.Next(1, width - 1);
            int y = rand.Next(1, height - 1);

            if (map[y, x] == '.')
            {
                map[y, x] = 'C';
                placed++;
            }
        }
    }

    private static void ReceiveData()
    {
        byte[] buffer = new byte[1024];
        int bytesRead;

        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
        {
            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            var parts = message.Split(' ');

            if (parts.Length == 2 &&
                int.TryParse(parts[0], out int x) &&
                int.TryParse(parts[1], out int y))
            {
                MovePlayer(clientPlayer, x, y);
                SendMap();
            }
        }
    }

    private static void HandleMove(ConsoleKey key, Player player)
    {
        int newX = player.X;
        int newY = player.Y;

        switch (key)
        {
            case ConsoleKey.LeftArrow:
            case ConsoleKey.A:
                newX--;
                break;
            case ConsoleKey.RightArrow:
            case ConsoleKey.D:
                newX++;
                break;
            case ConsoleKey.UpArrow:
            case ConsoleKey.W:
                newY--;
                break;
            case ConsoleKey.DownArrow:
            case ConsoleKey.S:
                newY++;
                break;
        }

        if (map[newY, newX] == '#')
            return; // wall

        // Chest
        if (map[newY, newX] == 'C')
        {
            player.Score += 10;
            map[newY, newX] = '.';
        }

        // Finish
        if (map[newY, newX] == 'F')
        {
            player.Finished = true;
            player.FinishTime = DateTime.Now;
        }

        map[player.Y, player.X] = '.';
        player.X = newX;
        player.Y = newY;
        map[player.Y, player.X] = player.Symbol[0];
    }

    private static void MovePlayer(Player player, int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
            return;

        if (map[y, x] == '#')
            return;

        // Chest
        if (map[y, x] == 'C')
        {
            player.Score += 10;
            map[y, x] = '.';
        }

        // Finish
        if (map[y, x] == 'F')
        {
            player.Finished = true;
            player.FinishTime = DateTime.Now;
        }

        map[player.Y, player.X] = '.';
        player.X = x;
        player.Y = y;
        map[player.Y, player.X] = player.Symbol[0];
    }

    private static void SendMap()
    {
        var sb = new StringBuilder();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
                sb.Append(map[y, x]);
            sb.AppendLine();
        }

        // HUD
        sb.AppendLine($"Time left: {(endTime - DateTime.Now).Seconds}s");
        int chestsLeft = 0;
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                if (map[y, x] == 'C') chestsLeft++;
        sb.AppendLine($"Chests left: {chestsLeft}");
        sb.AppendLine("Scores: " + string.Join("  ", players.Select(p => $"{p.Symbol}:{p.Score}")));
        sb.AppendLine("Goal: Finish first OR collect more treasures!");

        string mapString = sb.ToString();
        byte[] data = Encoding.UTF8.GetBytes(mapString);
        stream.Write(data, 0, data.Length);

        Console.Clear();
        Console.WriteLine(mapString);
    }

    private static void UpdateTimer(Object source, ElapsedEventArgs e)
    {
        if (DateTime.Now >= endTime)
        {
            timer.Stop();
            string result = GetWinner();

            Console.Clear();
            Console.WriteLine("Time is up!");
            Console.WriteLine(result);

            SendToClient(result);

            Environment.Exit(0);
        }
        else
        {
            SendMap();
        }
    }

    private static string GetWinner()
    {
        // Both finished
        if (players.All(p => p.Finished))
        {
            var fastest = players.OrderBy(p => p.FinishTime).First();
            return $"Winner: {fastest.Symbol} (faster to finish)";
        }

        // One finished
        if (players.Any(p => p.Finished))
        {
            var winner = players.First(p => p.Finished);
            return $"Winner: {winner.Symbol} (reached finish)";
        }

        // Nobody finished
        int maxScore = players.Max(p => p.Score);
        var leaders = players.Where(p => p.Score == maxScore).ToList();

        if (leaders.Count > 1)
            return "Draw! (equal scores)";
        else
            return $"Winner: {leaders[0].Symbol} (more treasures)";
    }

    private static void SendToClient(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        stream.Write(data, 0, data.Length);
    }


    public class Game
    {
        private const int WIN_SCORE = 50;
        private int score;
        private int totalChests;
        private DateTime startTime;
        private bool isRunning;

        public Game(int totalChests)
        {
            this.totalChests = totalChests;
            this.score = 0;
            this.isRunning = true;
            this.startTime = DateTime.Now;
        }

        public void AddScore(int points)
        {
            score += points;
            if (score >= WIN_SCORE)
            {
                EndGame(true);
            }
        }

        public void Update(TimeSpan remainingTime)
        {
            if (remainingTime.TotalSeconds <= 0)
            {
                EndGame(score >= WIN_SCORE);
            }
        }

        private void EndGame(bool isWin)
        {
            isRunning = false;
            Console.Clear();

            Console.ForegroundColor = isWin ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine(isWin
                ? @"
 __     ______  _    _   __          _______ _   _ 
 \ \   / / __ \| |  | |  \ \        / / ____| \ | |
  \ \_/ / |  | | |  | |   \ \  /\  / / (___ |  \| |
   \   /| |  | | |  | |    \ \/  \/ / \___ \| . ` |
    | | | |__| | |__| |     \  /\  /  ____) | |\  |
    |_|  \____/ \____/       \/  \/  |_____/|_| \_|
"
                : @"
  ____                         ___                 
 / ___| __ _ _ __ ___   ___   / _ \__   _____ _ __ 
| |  _ / _` | '_ ` _ \ / _ \ | | | \ \ / / _ \ '__|
| |_| | (_| | | | | | |  __/ | |_| |\ V /  __/ |   
 \____|\__,_|_| |_| |_|\___|  \___/  \_/ \___|_|   
");

            Console.ResetColor();

            Console.WriteLine($"\nYour score: {score} points");
            Console.WriteLine($"Total chests in labyrinth: {totalChests}");
            Console.WriteLine($"Time played: {(DateTime.Now - startTime).TotalSeconds:F1} sec\n");

            Console.WriteLine("Press [R] to restart or [Q] to quit.");
            HandleEndInput();
        }

        private void HandleEndInput()
        {
            while (true)
            {
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Q)
                {
                    Environment.Exit(0);
                }
                else if (key == ConsoleKey.R)
                {
                    Restart();
                    break;
                }
            }
        }

        private void Restart()
        {
            Console.Clear();
            Console.WriteLine("Restarting game...");
        }
    }



    

}

namespace Shared
{
    public enum TileType
    {
        Empty,   // ' '
        Wall,    // '#'
        Chest,   // 'C'
        Finish   // 'F'
    }

    public enum CommandType
    {
        Move
    }

    public class Command
    {
        public CommandType Type { get; set; }
        public string PlayerId { get; set; } = "";
        public string Data { get; set; } = "";
    }

    public class Position
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class Player
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int X { get; set; }
        public int Y { get; set; }
        public string Symbol { get; set; } = "0"; 

        public int Score { get; set; } = 0;

        public bool Finished { get; set; } = false;
        public DateTime? FinishTime { get; set; } = null;
    }
    public class GameMap
    {
        public TileType[,] Tiles { get; set; }

        public GameMap(int width, int height)
        {
            Tiles = new TileType[width, height];
        }
    }


    public class GameMessage
    {
        public string Type { get; set; } 
        public string Data { get; set; } 
    }

    public class GameState
    {
        public int ClientTreasures { get; set; }
        public int ServerTreasures { get; set; }
        public char[,] Map { get; set; }
        public object Client { get; set; }
        public object Server { get; set; }

        public string MapText { get; set; } = "";
        public int TimeLeft { get; set; } = 0; 
        public List<Player> Players { get; set; } = new List<Player>();
        public bool GameOver { get; set; } = false;
        public string ResultText { get; set; } = "";
    }

}


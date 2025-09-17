using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hitman_Simulator;

public class Logger : ILogger
{
    private static Logger _instance;
    private static readonly object _lock = new();

    private readonly string _filePath = "missions.log";

    public static Logger Instance
    {
        get
        {
            lock (_lock)
            {
                return _instance ??= new Logger();
            }
        }
    }

    private Logger() { }

    public void Log(string message)
    {
        var logMessage = $"[{DateTime.Now:HH:mm:ss}] {message}";
        Console.WriteLine(logMessage);
        File.AppendAllText(_filePath, logMessage + Environment.NewLine);
    }
}

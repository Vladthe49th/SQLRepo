using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}


// Custom logger
public class EfFileLogger : ILogger
{
    private readonly string _categoryName;
    private readonly LogLevel _minLogLevel;
    private readonly string _filePath;

    public EfFileLogger(string categoryName, LogLevel minLogLevel, string filePath)
    {
        _categoryName = categoryName;
        _minLogLevel = minLogLevel;
        _filePath = filePath;
    }

    public IDisposable BeginScope<TState>(TState state) => null!;

    public bool IsEnabled(LogLevel logLevel) => logLevel >= _minLogLevel;

    public void Log<TState>(LogLevel logLevel, EventId eventId,
        TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;

        var message = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] {_categoryName}: {formatter(state, exception)}";

        if (exception != null)
            message += Environment.NewLine + exception;

        File.AppendAllText(_filePath, message + Environment.NewLine);
    }
}


// Log Provider
public class EfFileLoggerProvider : ILoggerProvider
{
    private readonly LogLevel _minLogLevel;
    private readonly string _filePath;
    private readonly ConcurrentDictionary<string, EfFileLogger> _loggers = new();

    public EfFileLoggerProvider(LogLevel minLogLevel, string filePath)
    {
        _minLogLevel = minLogLevel;
        _filePath = filePath;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return _loggers.GetOrAdd(categoryName, name => new EfFileLogger(name, _minLogLevel, _filePath));
    }

    public void Dispose() => _loggers.Clear();
}

// Context

public class MyDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddProvider(new EfFileLoggerProvider(LogLevel.Information, "ef-log.txt"));
        });

        optionsBuilder
            .UseLoggerFactory(loggerFactory)
            .EnableSensitiveDataLogging(); // To be able to log SQL query paraameters
            
    }
}
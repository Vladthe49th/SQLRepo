


using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.IO;



namespace Entity_Home1
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<ApplicationDbContext>();
            context.Database.EnsureCreated();

            var trainService = services.GetRequiredService<TrainService>();



            trainService.AddTrain(new Train
            {
                Number = "IC-101",
                Departure = "Kyiv",
                Arrival = "Odessa",
                DepartureTime = DateTime.Now.AddHours(3),
                ArrivalTime = DateTime.Now.AddHours(9),
                SeatsAvailable = 110
            });


            Console.WriteLine("All trains:");
            foreach (var train in trainService.GetAllTrains())
            {
                Console.WriteLine($"{train.Id}: {train.Number} | {train.Departure} → {train.Arrival}");
            }


            trainService.UpdateTrain(1, new Train
            {
                Number = "IC-202",
                Departure = "Kyiv",
                Arrival = "Lviv",
                DepartureTime = DateTime.Now.AddHours(2),
                ArrivalTime = DateTime.Now.AddHours(7),
                SeatsAvailable = 98
            });


            trainService.DeleteTrain(1);
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    var connection = context.Configuration.GetConnectionString("DefaultConnection");
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlServer(connection));
                    services.AddTransient<TrainService>();
                });

    }
}

// Train class
public class Train
{
    public int Id { get; set; }
    public string Number { get; set; }
    public string Departure { get; set; }
    public string Arrival { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public int SeatsAvailable { get; set; }
}

// DbContext
public class ApplicationDbContext : DbContext
{
    public DbSet<Train> Trains { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
}
// Train service
public class TrainService
{
    private readonly ApplicationDbContext _context;

    public TrainService(ApplicationDbContext context)
    {
        _context = context;
    }


    // Add train
    public void AddTrain(Train train)
    {
        _context.Trains.Add(train);
        _context.SaveChanges();
    }

    public List<Train> GetAllTrains()
    {
        return _context.Trains.ToList();
    }

    // Update train
    public void UpdateTrain(int id, Train updatedTrain)
    {
        var train = _context.Trains.Find(id);
        if (train != null)
        {
            train.Number = updatedTrain.Number;
            train.Departure = updatedTrain.Departure;
            train.Arrival = updatedTrain.Arrival;
            train.DepartureTime = updatedTrain.DepartureTime;
            train.ArrivalTime = updatedTrain.ArrivalTime;
            train.SeatsAvailable = updatedTrain.SeatsAvailable;
            _context.SaveChanges();
        }
    }


    // Delete train
    public void DeleteTrain(int id)
    {
        var train = _context.Trains.Find(id);
        if (train != null)
        {
            _context.Trains.Remove(train);
            _context.SaveChanges();
        }
    }
}







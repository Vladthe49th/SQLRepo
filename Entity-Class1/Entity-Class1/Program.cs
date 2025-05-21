using Microsoft.EntityFrameworkCore;

public class DishMenu
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

public class AppDbContext : DbContext
{
    public DbSet<DishMenu> DishMenus { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=DishMenuDb;Trusted_Connection=True;");
    }
}

class Program
{
    static void Main()
    {
        using (var context = new AppDbContext())
        {
            context.Database.EnsureCreated(); 

            // Add mushroom soup
            var singleDish = new DishMenu { Name = "Mushroom soup", Price = 50 };
            context.DishMenus.Add(singleDish);
            context.SaveChanges();

            // Add several dishes
            var dishList = new List<DishMenu>
            {
                new DishMenu { Name = "Pizza with german sausages", Price = 40 },
                new DishMenu { Name = "Kotletki", Price = 35 },
                new DishMenu { Name = "Soup with chicken", Price = 45 }
            };

            context.DishMenus.AddRange(dishList);
            context.SaveChanges();
        }

        
        using (var context = new AppDbContext())
        {
            // Database connection check
            if (context.Database.CanConnect())
            {
                Console.WriteLine("Database accessible!");

                // Getting all "soup" dishes
                var soups = context.DishMenus
                    .Where(d => d.Name.Contains("Soup"))
                    .ToList();
                Console.WriteLine("Soups:");
                foreach (var dish in soups)
                    Console.WriteLine($"- {dish.Name} ({dish.Price} uah)");

                // Get a dish by Id
                int searchId = 1;
                var dishById = context.DishMenus.Find(searchId);
                Console.WriteLine($"\nYour dish by id - {searchId}: {dishById?.Name ?? "Not found!"}");

                // The last dish
                var lastDish = context.DishMenus
                    .OrderByDescending(d => d.Id)
                    .FirstOrDefault();
                Console.WriteLine($"\nThe last added dish: {lastDish?.Name ?? "No dishes at all!"}");
            }
            else
            {
                Console.WriteLine("Database unaccessible!");
            }
        }
    }
}
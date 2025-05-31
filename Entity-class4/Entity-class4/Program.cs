using Microsoft.EntityFrameworkCore;

public class Shop
{
    public int Id { get; set; }
    public string Name { get; set; }

    //One shop - many games
    public List<Game> Games { get; set; } = new();
}

public class Game
{
    public int Id { get; set; }
    public string Title { get; set; }

    // Outer key
    public int ShopId { get; set; }

    //  Navigation property
    public Shop Shop { get; set; }
}


public class GameDbContext : DbContext
{
    public DbSet<Shop> Shops { get; set; }
    public DbSet<Game> Games { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("GameStoreDb");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Shop>()
            .HasMany(s => s.Games)
            .WithOne(g => g.Shop)
            .HasForeignKey(g => g.ShopId);
    }
}


class Program
{
    static void Main(string[] args)
    {
        using var context = new GameDbContext();

        // Making some shops
        var shop1 = new Shop { Name = "GameWorld" };
        var shop2 = new Shop { Name = "PixelPlanet" };

        // 1) Install the main entity through navigation property of dependent entity
        var game1 = new Game { Title = "Mass Effect", Shop = shop1 };

        // Adding shops and the first game
        context.Shops.Add(shop1);
        context.Shops.Add(shop2);
        context.Games.Add(game1);
        context.SaveChanges();

        // 2) Installing through outer key
        var game2 = new Game { Title = "The Witcher", ShopId = shop1.Id };
        context.Games.Add(game2);

        // 3) Navigation property of main entity
        var game3 = new Game { Title = "God of War" };
        shop2.Games.Add(game3);

        context.SaveChanges();
    }
}

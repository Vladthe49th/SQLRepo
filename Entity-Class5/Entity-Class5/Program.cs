using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;


// City class
public class City
{
    public int Id { get; set; }
    public string Name { get; set; }

    // Navigation
    public ICollection<Shop> Shops { get; set; } = new List<Shop>();
}

// Shop class
public class Shop
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }

    public int CityId { get; set; }
    public City City { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<Supplier> Suppliers { get; set; } = new List<Supplier>();


}

// Supplier class

public class Supplier
{
    public int Id { get; set; }
    public string Name { get; set; }

    public ICollection<Shop> Shops { get; set; } = new List<Shop>();


}


// Product class
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Cost { get; set; }
    public int Quantity { get; set; }

    public int ShopId { get; set; }
    public Shop Shop { get; set; }
}


// Context

public class FlowerShopContext : DbContext
{
    public DbSet<City> Cities { get; set; }
    public DbSet<Shop> Shops { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }

    public FlowerShopContext(DbContextOptions<FlowerShopContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Shop>()
            .HasMany(s => s.Suppliers)
            .WithMany(s => s.Shops)
            .UsingEntity(j => j.ToTable("ShopSuppliers"));
    }
}


// Initializer
public static class DbInitializer
{
    public static void Initialize(FlowerShopContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        var kyiv = new City { Name = "Kyiv" };
        var dnipro = new City { Name = "Dnipro" };

        var shop1 = new Shop { Name = "Poison Ivy", Address = "Lesya Ukrainka st.12", City = kyiv };
        var shop2 = new Shop { Name = "Feeria", Address = "Kreshatik 33", City = kyiv };
        var shop3 = new Shop { Name = "Tuulpaannnn", Address = "Yavornicka 88", City = dnipro };
        var shop4 = new Shop { Name = "In the power of Baba Galya", Address = "No return 11", City = dnipro };

        var supplier1 = new Supplier { Name = "Supplier А" };
        var supplier2 = new Supplier { Name = "Supplier not A" };

        shop1.Suppliers.Add(supplier1);
        shop2.Suppliers.Add(supplier2);
        shop3.Suppliers.Add(supplier1);
        shop4.Suppliers.Add(supplier2);

        var products = new List<Product>
        {
            new Product { Name = "Rose", Cost = 100, Quantity = 10, Shop = shop1 },
            new Product { Name = "Tulpan", Cost = 70, Quantity = 15, Shop = shop1 },
            new Product { Name = "Lili", Cost = 120, Quantity = 5, Shop = shop2 },
            new Product { Name = "Chrisantema", Cost = 90, Quantity = 20, Shop = shop3 },
            new Product { Name = "Devil`s flower", Cost = 50, Quantity = 8, Shop = shop4 },
            new Product { Name = "Gvozdika", Cost = 60, Quantity = 12, Shop = shop4 },
        };

        context.Cities.AddRange(kyiv, dnipro);
        context.Shops.AddRange(shop1, shop2, shop3, shop4);
        context.Suppliers.AddRange(supplier1, supplier2);
        context.Products.AddRange(products);

        context.SaveChanges();
    }
}


class Program
{
    static void Main(string[] args)
    {
        var options = new DbContextOptionsBuilder<FlowerShopContext>()
       .UseInMemoryDatabase("FlowerShopDb") 
       .Options;

        using var context = new FlowerShopContext(options);
        DbInitializer.Initialize(context);



        // 1) Search product by certain shop
        string searchTerm = "Rose";
        int shopId = 1;

        var productsInShop = context.Products
            .Where(p => p.ShopId == shopId && EF.Functions.Like(p.Name.ToLower(), $"%{searchTerm.ToLower()}%"))
            .ToList();

        foreach (var p in productsInShop)
        {
            Console.WriteLine($"{p.Name} — {p.Cost} uah, total num: {p.Quantity}");
        }



        // 2) Search product by all shops

        string searchAll = "Tulpan";

        var products = context.Products
            .Where(p => EF.Functions.Like(p.Name.ToLower(), $"%{searchAll.ToLower()}%"))
            .Include(p => p.Shop)
            .ToList();

        foreach (var p in products)
        {
            Console.WriteLine($"{p.Name} — {p.Cost} uah ({p.Shop.Name})");
        }


        // 3) Random product from a shop

        int targetShopId = 1;

        var randomProduct = context.Products
            .Where(p => p.ShopId == targetShopId)
            .OrderBy(p => Guid.NewGuid()) 
            .FirstOrDefault();

        if (randomProduct != null)
            Console.WriteLine($"Random product: {randomProduct.Name} — {randomProduct.Cost} uah");


        //4)  Sort by price

        Console.WriteLine("By increasing:");
        var asc = context.Products.OrderBy(p => p.Cost).ToList();
        asc.ForEach(p => Console.WriteLine($"{p.Name} — {p.Cost} uah"));

        Console.WriteLine("\n By decreasing:");
        var desc = context.Products.OrderByDescending(p => p.Cost).ToList();
        desc.ForEach(p => Console.WriteLine($"{p.Name} — {p.Cost} uah"));



        //5)  Number and sum of products in certain city

        int cityId = 1; 

        var report = context.Shops
            .Where(s => s.CityId == cityId)
            .Select(s => new
            {
                ShopName = s.Name,
                TotalQuantity = s.Products.Sum(p => p.Quantity),
                TotalValue = s.Products.Sum(p => p.Quantity * p.Cost)
            })
            .ToList();

        foreach (var item in report)
        {
            Console.WriteLine($"{item.ShopName}: Number = {item.TotalQuantity}, sum = {item.TotalValue} uah");
        }


        //6)  Common information

        var cities = context.Cities
    .Include(c => c.Shops)
        .ThenInclude(s => s.Products)
    .ToList();

        foreach (var city in cities)
        {
            Console.WriteLine($"-- {city.Name}");
            foreach (var shop in city.Shops)
            {
                Console.WriteLine($"   -- {shop.Name}");
                foreach (var product in shop.Products)
                {
                    Console.WriteLine($" -- {product.Name}: {product.Cost} uah, number - {product.Quantity}");
                }
            }
        }


        //7)  Products with sum more expensive than 500

        int storeId = 4;
        decimal minTotalValue = 500;

        var expensiveProducts = context.Products
            .Where(p => p.ShopId == storeId && p.Cost * p.Quantity > minTotalValue)
            .ToList();

        foreach (var p in expensiveProducts)
        {
            Console.WriteLine($"{p.Name}: {p.Cost} uah x number of {p.Quantity} = {p.Cost * p.Quantity} uah");
        }



        // 8) Shops with more than 25 products

        int minTotalQuantity = 25;

        var shops = context.Shops
            .Where(s => s.Products.Sum(p => p.Quantity) > minTotalQuantity)
            .ToList();

        foreach (var shop in shops)
        {
            Console.WriteLine($"{shop.Name}");
        }




        // 9)  Get shops through city

        var city = context.Cities
    .Include(c => c.Shops)
    .FirstOrDefault(c => c.Id == 1);

        if (city != null)
        {
            Console.WriteLine($"Shops in the city {city.Name}:");
            foreach (var shop in city.Shops)
                Console.WriteLine($"- {shop.Name}");
        }


        // 10)  Average prices for each shop

        var avgPrices = context.Cities
    .Include(c => c.Shops)
        .ThenInclude(s => s.Products)
    .ToList();

        foreach (var city in avgPrices)
        {
            Console.WriteLine($"{city.Name}:");
            foreach (var shop in city.Shops)
            {
                var avg = shop.Products.Any() ? shop.Products.Average(p => p.Cost) : 0;
                Console.WriteLine($"Shop \"{shop.Name}\": : {avg:F2}");
            }
        }



    }



}


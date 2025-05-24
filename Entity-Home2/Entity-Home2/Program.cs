


using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;


namespace Entity_Home2
{

    //Product class
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }

        // Сonnect to orders
        public List<Order>? Orders { get; set; }
    }



    // Order class

    public class Order
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }

        public List<Product>? Products { get; set; }
    }

    // Shop context
    public class ShopContext : DbContext
    {
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Order> Orders => Set<Order>();

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ShopDb;Trusted_Connection=True;");
        }
    }


    // Services
    public class OrderService
    {
        private readonly ShopContext _context;

        public OrderService(ShopContext context)
        {
            _context = context;
        }

        // Create order
        public void CreateOrder(List<int> productIds)
        {
            var products = _context.Products.Where(p => productIds.Contains(p.ProductId)).ToList();
            var order = new Order
            {
                OrderDate = DateTime.Now,
                Products = products
            };
            _context.Orders.Add(order);
            _context.SaveChanges();
        }

        // Delete order
        public void DeleteOrder(int orderId)
        {
            var order = _context.Orders.Include(o => o.Products).FirstOrDefault(o => o.OrderId == orderId);
            if (order != null)
            {
                _context.Orders.Remove(order);
                _context.SaveChanges();
            }
        }

        // Get orders
        public List<Order> GetOrders()
        {
            return _context.Orders.Include(o => o.Products).ToList();
        }
    }



    internal class Program
    {
        static void Main(string[] args)
        {
            using var db = new ShopContext();

            // Products
            if (!db.Products.Any())
            {
                db.Products.AddRange(
                    new Product { Name = "Milk", Price = 30 },
                    new Product { Name = "Bread", Price = 20 },
                    new Product { Name = "Cigarettes", Price = 50 }
                );
                db.SaveChanges();
                Console.WriteLine("Products added!");
            }

            var service = new OrderService(db);

            // Making an order
            service.CreateOrder(new List<int> { 1, 2 });

            // Order list
            var orders = service.GetOrders();
            foreach (var order in orders)
            {
                Console.WriteLine($"Order №{order.OrderId}, Date: {order.OrderDate}");
                foreach (var product in order.Products!)
                    Console.WriteLine($"  - {product.Name}, {product.Price} uah");
            }

            // Delete the first order
            if (orders.Any())
            {
                service.DeleteOrder(orders.First().OrderId);
                Console.WriteLine("First order deleted.");
            }

        }
    }
}

using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;

namespace Entity_Home9
{

    // Class Author

    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Book> Books { get; set; }
    }


    // Class Category
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Book> Books { get; set; }
    }


    // Class Book
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public DateTime ReleaseDate { get; set; }

        public int AuthorId { get; set; }
        public Author Author { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }

    
    // Customer class
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }


    // Class Rating
    public class Rating
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int BookId { get; set; }
        public int Score { get; set; }
    }


    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
    }



    // Cart item class

    public class CartItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }


    // Cart service class
    public class CartService
    {
        private List<CartItem> cartItems = new();

        // Add to cart
        public void AddToCart(Product product, int quantity)
        {
            var existing = cartItems.FirstOrDefault(ci => ci.Product.Id == product.Id);
            if (existing != null)
                existing.Quantity += quantity;
            else
                cartItems.Add(new CartItem { Product = product, Quantity = quantity });
        }


        // View cart

        public void ViewCart()
        {
            foreach (var item in cartItems)
            {
                Console.WriteLine($"{item.Product.Name} x {item.Quantity} = ${item.Product.Price * item.Quantity:F2}");
            }
        }

        // Checkout

        public decimal Checkout(ProductRepository repo)
        {
            decimal total = 0;
            foreach (var item in cartItems)
            {
                total += item.Product.Price * item.Quantity;
                item.Product.Stock -= item.Quantity;
                repo.UpdateProduct(item.Product);
            }

            cartItems.Clear();
            return total;
        }

        internal List<CartItem> GetItems()
        {
            throw new NotImplementedException();
        }
    }


    // Order service





    // Context

    public class LibraryContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=CompanyDb;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>().HasMany(a => a.Books).WithOne(b => b.Author);
            modelBuilder.Entity<Category>().HasMany(c => c.Books).WithOne(b => b.Category);
        }
    }






    internal class Program
    {

        // 1) Get books of a specific author

        public void GetBooksByAuthor(string authorName)
        {
            using var context = new LibraryContext();
            var books = context.Books
                .Where(b => b.Author.Name == authorName)
                .Select(b => new
                {
                    b.Title,
                    AuthorName = b.Author.Name,
                    CategoryName = b.Category.Name,
                    b.Price,
                    b.ReleaseDate
                })
                .ToList();

            foreach (var book in books)
            {
                Console.WriteLine($"{book.Title} - {book.AuthorName}, {book.CategoryName}, ${book.Price}, {book.ReleaseDate:d}");
            }
        }


        // 2) Delete cheapest book in category

        public void DeleteCheapestBookInCategory(string categoryName)
        {
            using var context = new LibraryContext();
            var bookToDelete = context.Books
                .Where(b => b.Category.Name == categoryName)
                .OrderBy(b => b.Price)
                .FirstOrDefault();

            if (bookToDelete != null)
            {
                context.Books.Remove(bookToDelete);
                context.SaveChanges();
            }
        }


        //3) Increase book prices by 5%

        public void IncreaseAllBookPricesBy5Percent()
        {
            using var context = new LibraryContext();
            var books = context.Books.ToList();

            foreach (var book in books)
            {
                book.Price *= 1.05m;
            }

            context.SaveChanges();
        }

        //4) Books in a certain price range

        public void GetBooksInPriceRange(decimal minPrice, decimal maxPrice)
        {
            using var context = new LibraryContext();
            var books = context.Books
                .Where(b => b.Price >= minPrice && b.Price <= maxPrice)
                .Select(b => new { b.Title, b.Price })
                .ToList();

            foreach (var book in books)
            {
                Console.WriteLine($"{book.Title} - ${book.Price}");
            }
        }

        //5) Books count for each author

        public void GetBooksCountByAuthor()
        {
            using var context = new LibraryContext();
            var result = context.Authors
                .Select(a => new
                {
                    AuthorId = a.Id,
                    AuthorName = a.Name,
                    BooksCount = a.Books.Count
                })
                .ToList();

            foreach (var entry in result)
            {
                Console.WriteLine($"ID: {entry.AuthorId}, Name: {entry.AuthorName}, number of books: {entry.BooksCount}");
            }
        }



        static void Main(string[] args)
        {
            static void Main(string[] args)
            {
                var ratingRepo = new BookRatingRepository();

                while (true)
                {
                    Console.WriteLine("\n--- Book rating system ---");
                    Console.WriteLine("1. Add rating to a book");
                    Console.WriteLine("2. Get average rating of a book");
                    Console.WriteLine("3. Top-5 books by ratings");
                    Console.WriteLine("0. Exit");

                    Console.Write("Choose your option: ");
                    var choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            Console.Write("User ID: ");
                            int customerId = int.Parse(Console.ReadLine());

                            Console.Write("Book ID: ");
                            int bookId = int.Parse(Console.ReadLine());

                            Console.Write("Rating(1-5): ");
                            int score = int.Parse(Console.ReadLine());

                            ratingRepo.AddRating(customerId, bookId, score);
                            Console.WriteLine("Rating added!");
                            break;

                        case "2":
                            Console.Write("Book ID: ");
                            int bookIdAvg = int.Parse(Console.ReadLine());

                            double avg = ratingRepo.GetAverageRating(bookIdAvg);
                            Console.WriteLine($"Average rating for this book is: {avg:F2}");
                            break;

                        case "3":
                            var topBooks = ratingRepo.GetTop5RatedBooks();
                            Console.WriteLine("Top-5 books by rating:");
                            foreach (var book in topBooks)
                            {
                                Console.WriteLine($"ID: {book.BookId}, Name: {book.Title}, rating: {book.AvgRating:F2}");
                            }
                            break;

                        case "0":
                            return;

                        default:
                            Console.WriteLine("Wrong choice, now die!");
                            break;
                    }
                }


                //Online store


                var productRepo = new ProductRepository();
                var cart = new CartService();
                var orderService = new OrderService();
                var reviewService = new ReviewService();

                while (true)
                {
                    Console.WriteLine("\n--- Online store ---");
                    Console.WriteLine("1. Show all products");
                    Console.WriteLine("2. Add product to cart");
                    Console.WriteLine("3. View cart ");
                    Console.WriteLine("4. Make an order");
                    Console.WriteLine("5. Total Revenue");
                    Console.WriteLine("6. Top selling products");
                    Console.WriteLine("7. Leave a review");
                    Console.WriteLine("8. See reviews");
                    Console.WriteLine("0. Go back");

                    Console.Write("Your choice: ");
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            var products = productRepo.GetAllProducts();
                            foreach (var p in products)
                                Console.WriteLine($"{p.Id}: {p.Name} - ${p.Price} | Leftover: {p.Stock}");
                            break;

                        case "2":
                            Console.Write("Product ID: ");
                            int prodId = int.Parse(Console.ReadLine());
                            Console.Write("Number: ");
                            int qty = int.Parse(Console.ReadLine());
                            var product = productRepo.GetAllProducts().FirstOrDefault(p => p.Id == prodId);
                            if (product != null)
                            {
                                cart.AddToCart(product, qty);
                                Console.WriteLine("Product added!");
                            }
                            else
                            {
                                Console.WriteLine("Продукт не найден.");
                            }
                            break;

                        case "3":
                            Console.WriteLine("Cart content:");
                            cart.ViewCart();
                            break;

                        case "4":
                            try
                            {
                                var cartItems = cart.GetItems();
                                if (!cartItems.Any())
                                {
                                    Console.WriteLine("Cart empty!");
                                    break;
                                }

                                int orderId = orderService.CreateOrder(cartItems);
                                Console.WriteLine($"Order #{orderId} made successfully");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Order-making mistake: " + ex.Message);
                            }
                            break;

                        case "5":
                            decimal revenue = orderService.GetTotalRevenue();
                            Console.WriteLine($"Total revenue: ${revenue:F2}");
                            break;

                        case "6":
                            Console.WriteLine("Top selling products:");
                            var top = orderService.GetTopSellingProducts();
                            foreach (var (Name, Quantity) in top)
                                Console.WriteLine($"{Name}: sold {Quantity} copies");
                            break;

                        case "7":
                            Console.Write("Product ID: ");
                            int reviewProdId = int.Parse(Console.ReadLine());
                            Console.Write("User name: ");
                            string user = Console.ReadLine();
                            Console.Write("Commentary: ");
                            string comment = Console.ReadLine();
                            reviewService.AddReview(reviewProdId, user, comment);
                            Console.WriteLine("Review made!");
                            break;

                        case "8":
                            Console.Write("Product ID: ");
                            int revId = int.Parse(Console.ReadLine());
                            var reviews = reviewService.GetReviews(revId);
                            foreach (var r in reviews)
                                Console.WriteLine($"{r.CustomerName} ({r.CreatedAt}): {r.Comment}");
                            break;

                        case "0":
                            return;

                        default:
                            Console.WriteLine("Wrong choice, now die!");
                            break;
                    }
                }


            }
        }
    }
}




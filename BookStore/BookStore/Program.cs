using BookStore.Data;
using BookStore.Interfaces;
using BookStore.Models;
using BookStore.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BookStore
{
    class Program
    {
        static async Task Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("========= MAIN MENU =========");
                Console.WriteLine("1. Manage Books");
                Console.WriteLine("2. Manage Authors");
                Console.WriteLine("3. Manage Categories");
                Console.WriteLine("4. Manage Promotions");
                Console.WriteLine("5. Manage Orders");
                Console.WriteLine("6. Manage Reviews");
                Console.WriteLine("7. Exit");
                Console.WriteLine("============================");
                Console.Write("Select an option: ");
                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        await new BookMenu().ShowAsync();
                        break;
                    case "2":
                        await new AuthorMenu().ShowAsync();
                        break;
                    case "3":
                        await new CategoryMenu().ShowAsync();
                        break;
                    case "4":
                        await new PromotionMenu().ShowAsync();
                        break;
                    case "5":
                        await new OrderMenu().ShowAsync();
                        break;
                    case "6":
                        await new ReviewMenu().ShowAsync();
                        break;
                    case "7":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Press Enter to try again.");
                        Console.ReadLine();
                        break;
                }
            }
        }

        public class BookMenu
        {
            private readonly BookRepository _bookRepo = new();

            public async Task ShowAsync()
            {
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("=== BOOK MENU ===");
                    Console.WriteLine("1. Show all books");
                    Console.WriteLine("2. Search books by name");
                    Console.WriteLine("3. View book details");
                    Console.WriteLine("4. Add new book");
                    Console.WriteLine("5. Edit book");
                    Console.WriteLine("6. Delete book");
                    Console.WriteLine("0. Back to main menu");

                    Console.Write("Select option: ");
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            await ShowAllBooks();
                            break;
                        case "2":
                            await SearchBooksByName();
                            break;
                        case "3":
                            await ViewBookDetails();
                            break;
                        case "4":
                            await AddBook();
                            break;
                        case "5":
                            await EditBook();
                            break;
                        case "6":
                            await DeleteBook();
                            break;
                        case "0":
                            return;
                        default:
                            Console.WriteLine("Invalid option. Try again.");
                            break;
                    }

                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }

            private async Task ShowAllBooks()
            {
                var books = await _bookRepo.GetAllBooksAsync();
                Console.WriteLine("\n=== All Books ===");
                foreach (var book in books)
                {
                    Console.WriteLine($"{book.Id}: {book.Name} | {book.PublishedOn:yyyy-MM-dd} | {book.Price:C}");
                }
            }

            private async Task SearchBooksByName()
            {
                Console.Write("Enter part of the book name: ");
                string name = Console.ReadLine();

                var books = await _bookRepo.GetBooksByNameAsync(name);
                Console.WriteLine($"\n=== Search Results for '{name}' ===");
                foreach (var book in books)
                {
                    Console.WriteLine($"{book.Id}: {book.Name}");
                }
            }

            private async Task ViewBookDetails()
            {
                Console.Write("Enter Book ID: ");
                if (int.TryParse(Console.ReadLine(), out int id))
                {
                    var book = await _bookRepo.GetBookWithAuthorsAndReviewAndCategoryAsync(id);
                    if (book == null)
                    {
                        Console.WriteLine("Book not found.");
                        return;
                    }

                    Console.WriteLine($"\n=== {book.Name} ===");
                    Console.WriteLine($"Description: {book.Description}");
                    Console.WriteLine($"Published On: {book.PublishedOn:yyyy-MM-dd}");
                    Console.WriteLine($"Publisher: {book.Publisher}");
                    Console.WriteLine($"Price: {book.Price:C}");
                    Console.WriteLine($"Category: {book.Category?.Name}");
                    Console.WriteLine("Authors: " + string.Join(", ", book.Authors.Select(a => a.Name)));
                    Console.WriteLine("Reviews: ");
                    foreach (var review in book.Reviews)
                    {
                        Console.WriteLine($" - {review.Comment}");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid ID.");
                }
            }

            private async Task AddBook()
            {
                Console.Write("Enter Book Name: ");
                string name = Console.ReadLine();

                Console.Write("Enter Description: ");
                string description = Console.ReadLine();

                Console.Write("Enter Published Date (yyyy-MM-dd): ");
                DateTime publishedOn = DateTime.Parse(Console.ReadLine());

                Console.Write("Enter Publisher: ");
                string publisher = Console.ReadLine();

                Console.Write("Enter Price: ");
                decimal price = decimal.Parse(Console.ReadLine());

                Console.Write("Enter Category ID: ");
                int categoryId = int.Parse(Console.ReadLine());

                var book = new Book
                {
                    Name = name,
                    Description = description,
                    PublishedOn = publishedOn,
                    Publisher = publisher,
                    Price = price,
                    CategoryId = categoryId,
                    Authors = new List<Author>() 
                };

                await _bookRepo.AddBookAsync(book);
                Console.WriteLine("Book added successfully.");
            }

            private async Task EditBook()
            {
                Console.Write("Enter Book ID to edit: ");
                int id = int.Parse(Console.ReadLine());

                var book = await _bookRepo.GetBookWithAuthorsAsync(id);
                if (book == null)
                {
                    Console.WriteLine("Book not found.");
                    return;
                }

                Console.Write("New Name (leave empty to keep current): ");
                string name = Console.ReadLine();
                if (!string.IsNullOrEmpty(name)) book.Name = name;

                Console.Write("New Description (leave empty to keep current): ");
                string description = Console.ReadLine();
                if (!string.IsNullOrEmpty(description)) book.Description = description;

                await _bookRepo.EditBookAsync(book);
                Console.WriteLine("Book updated.");
            }

            private async Task DeleteBook()
            {
                Console.Write("Enter Book ID to delete: ");
                int id = int.Parse(Console.ReadLine());

                var book = await _bookRepo.GetBookAsync(id);
                if (book == null)
                {
                    Console.WriteLine("Book not found.");
                    return;
                }

                await _bookRepo.DeleteBookAsync(book);
                Console.WriteLine("Book deleted.");
            }
        }


        public class AuthorMenu
        {
            private readonly AuthorRepository _repository = new();

            public async Task ShowAsync()
            {
                string choice;
                do
                {
                    Console.Clear();
                    Console.WriteLine("--- Author Menu ---");
                    Console.WriteLine("1. View all authors");
                    Console.WriteLine("2. View author by ID");
                    Console.WriteLine("3. Search authors by name");
                    Console.WriteLine("4. Add author");
                    Console.WriteLine("5. Edit author");
                    Console.WriteLine("6. Delete author");
                    Console.WriteLine("0. Return to main menu");
                    Console.Write("Select option: ");
                    choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            var all = await _repository.GetAllAuthorsAsync();
                            foreach (var a in all) Console.WriteLine($"{a.Id}. {a.Name}");
                            break;
                        case "2":
                            Console.Write("Enter ID: ");
                            if (int.TryParse(Console.ReadLine(), out int id))
                            {
                                var author = await _repository.GetAuthorWithBooksAsync(id);
                                if (author == null) Console.WriteLine("Author not found");
                                else
                                {
                                    Console.WriteLine($"{author.Name} (ID: {author.Id})");
                                    Console.WriteLine("Books:");
                                    foreach (var book in author.Books) Console.WriteLine(" - " + book.Name);
                                }
                            }
                            break;
                        case "3":
                            Console.Write("Enter name: ");
                            var search = await _repository.GetAuthorsByName(Console.ReadLine());
                            foreach (var a in search) Console.WriteLine($"{a.Id}. {a.Name}");
                            break;
                        case "4":
                            Console.Write("Author name: ");
                            string name = Console.ReadLine();
                            await _repository.AddAuthorAsync(new Author { Name = name });
                            Console.WriteLine("Author added.");
                            break;
                        case "5":
                            Console.Write("Enter ID: ");
                            if (int.TryParse(Console.ReadLine(), out int eid))
                            {
                                var author = await _repository.GetAuthorAsync(eid);
                                if (author != null)
                                {
                                    Console.Write("New name: ");
                                    author.Name = Console.ReadLine();
                                    await _repository.EditAuthorAsync(author);
                                    Console.WriteLine("Author updated.");
                                }
                            }
                            break;
                        case "6":
                            Console.Write("Enter ID: ");
                            if (int.TryParse(Console.ReadLine(), out int did))
                            {
                                var author = await _repository.GetAuthorAsync(did);
                                if (author != null)
                                {
                                    await _repository.DeleteAuthorAsync(author);
                                    Console.WriteLine("Author deleted.");
                                }
                            }
                            break;
                    }

                    if (choice != "0")
                    {
                        Console.WriteLine("\nPress Enter to continue...");
                        Console.ReadLine();
                    }

                } while (choice != "0");
            }
        }

        public class CategoryMenu
        {
            private readonly CategoryRepository _repository = new();

            public async Task ShowAsync()
            {
                string choice;
                do
                {
                    Console.Clear();
                    Console.WriteLine("--- Category Menu ---");
                    Console.WriteLine("1. View all categories");
                    Console.WriteLine("2. View category by ID");
                    Console.WriteLine("3. Search categories by name");
                    Console.WriteLine("4. Add category");
                    Console.WriteLine("5. Edit category");
                    Console.WriteLine("6. Delete category");
                    Console.WriteLine("0. Return to main menu");
                    Console.Write("Select option: ");
                    choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            var all = await _repository.GetAllCategoriesAsync();
                            foreach (var c in all) Console.WriteLine($"{c.Id}. {c.Name} - {c.Description}");
                            break;
                        case "2":
                            Console.Write("Enter ID: ");
                            if (int.TryParse(Console.ReadLine(), out int id))
                            {
                                var category = await _repository.GetCategoryWithBooksAsync(id);
                                if (category == null) Console.WriteLine("Category not found");
                                else
                                {
                                    Console.WriteLine($"{category.Name} (ID: {category.Id}) - {category.Description}");
                                    Console.WriteLine("Books:");
                                    foreach (var book in category.Books) Console.WriteLine(" - " + book.Name);
                                }
                            }
                            break;
                        case "3":
                            Console.Write("Enter name: ");
                            var search = await _repository.GetCategoriesByNameAsync(Console.ReadLine());
                            foreach (var c in search) Console.WriteLine($"{c.Id}. {c.Name} - {c.Description}");
                            break;
                        case "4":
                            Console.Write("Category name: ");
                            string name = Console.ReadLine();
                            Console.Write("Description: ");
                            string desc = Console.ReadLine();
                            await _repository.AddCategoryAsync(new Category { Name = name, Description = desc });
                            Console.WriteLine("Category added.");
                            break;
                        case "5":
                            Console.Write("Enter ID: ");
                            if (int.TryParse(Console.ReadLine(), out int eid))
                            {
                                var category = await _repository.GetCategoryAsync(eid);
                                if (category != null)
                                {
                                    Console.Write("New name: ");
                                    category.Name = Console.ReadLine();
                                    Console.Write("New description: ");
                                    category.Description = Console.ReadLine();
                                    await _repository.UpdateCategoryAsync(category);
                                    Console.WriteLine("Category updated.");
                                }
                            }
                            break;
                        case "6":
                            Console.Write("Enter ID: ");
                            if (int.TryParse(Console.ReadLine(), out int did))
                            {
                                var category = await _repository.GetCategoryAsync(did);
                                if (category != null)
                                {
                                    await _repository.DeleteCategoryAsync(category);
                                    Console.WriteLine("Category deleted.");
                                }
                            }
                            break;
                    }

                    if (choice != "0")
                    {
                        Console.WriteLine("\nPress Enter to continue...");
                        Console.ReadLine();
                    }

                } while (choice != "0");
            }
        }

        public class PromotionMenu
        {
            private readonly IPromotion _promotionRepository = new PromotionRepository();

            public async Task ShowAsync()
            {
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("-- Promotion Menu --");
                    Console.WriteLine("1. View all promotions");
                    Console.WriteLine("2. Add promotion");
                    Console.WriteLine("3. Edit promotion");
                    Console.WriteLine("4. Delete promotion");
                    Console.WriteLine("0. Back");
                    Console.Write("Select: ");
                    switch (Console.ReadLine())
                    {
                        case "1":
                            var promotions = await _promotionRepository.GetAllPromotionsAsync();
                            foreach (var promo in promotions)
                                Console.WriteLine(promo);
                            break;
                        case "2":
                            Promotion newPromo = new();
                            Console.Write("Name: ");
                            newPromo.Name = Console.ReadLine();
                            Console.Write("Discount Percent (leave empty if using Amount): ");
                            var percentInput = Console.ReadLine();
                            if (!string.IsNullOrWhiteSpace(percentInput))
                                newPromo.Percent = decimal.Parse(percentInput);
                            else
                            {
                                Console.Write("Discount Amount: ");
                                newPromo.Amount = decimal.Parse(Console.ReadLine());
                            }
                            Console.Write("BookId: ");
                            newPromo.BookId = int.Parse(Console.ReadLine());
                            await _promotionRepository.AddPromotionAsync(newPromo);
                            break;
                        case "3":
                            Console.Write("Enter ID to edit: ");
                            int id = int.Parse(Console.ReadLine());
                            var promoEdit = await _promotionRepository.GetPromotionAsync(id);
                            if (promoEdit != null)
                            {
                                Console.Write("New Name: ");
                                promoEdit.Name = Console.ReadLine();
                                Console.Write("New Percent (leave empty if using Amount): ");
                                string percent = Console.ReadLine();
                                if (!string.IsNullOrWhiteSpace(percent))
                                {
                                    promoEdit.Percent = decimal.Parse(percent);
                                    promoEdit.Amount = null;
                                }
                                else
                                {
                                    Console.Write("New Amount: ");
                                    promoEdit.Amount = decimal.Parse(Console.ReadLine());
                                    promoEdit.Percent = null;
                                }
                                await _promotionRepository.EditPromotionAsync(promoEdit);
                            }
                            break;
                        case "4":
                            Console.Write("Enter ID to delete: ");
                            int delId = int.Parse(Console.ReadLine());
                            var promoDel = await _promotionRepository.GetPromotionAsync(delId);
                            if (promoDel != null)
                                await _promotionRepository.DeletePromotionAsync(promoDel);
                            break;
                        case "0": return;
                    }
                    Console.WriteLine("Press Enter to continue...");
                    Console.ReadLine();
                }
            }
        }


        public class OrderMenu
        {
            private readonly IOrder _orderRepository = new OrderRepository();

            public async Task ShowAsync()
            {
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("-- Order Menu --");
                    Console.WriteLine("1. View all orders");
                    Console.WriteLine("2. Search order by ID");
                    Console.WriteLine("3. Search orders by customer name");
                    Console.WriteLine("4. Add order");
                    Console.WriteLine("5. Update order");
                    Console.WriteLine("6. Delete order");
                    Console.WriteLine("0. Back");
                    Console.Write("Select: ");
                    switch (Console.ReadLine())
                    {
                        case "1":
                            var orders = await _orderRepository.GetAllOrdersAsync();
                            foreach (var order in orders)
                            {
                                Console.WriteLine($"ID: {order.Id}, Customer: {order.CustomerName}, Shipped: {order.Shipped}");
                            }
                            break;
                        case "2":
                            Console.Write("Enter ID: ");
                            int.TryParse(Console.ReadLine(), out int id);
                            var found = await _orderRepository.GetOrderAsync(id);
                            if (found != null)
                                Console.WriteLine($"Order #{found.Id} for {found.CustomerName}, Shipped: {found.Shipped}");
                            break;
                        case "3":
                            Console.Write("Enter customer name: ");
                            string name = Console.ReadLine();
                            var byName = await _orderRepository.GetAllOrdersByNameAsync(name);
                            foreach (var order in byName)
                                Console.WriteLine($"ID: {order.Id}, Customer: {order.CustomerName}, Shipped: {order.Shipped}");
                            break;
                        case "4":
                            Order newOrder = new();
                            Console.Write("Customer name: ");
                            newOrder.CustomerName = Console.ReadLine();
                            Console.Write("City: ");
                            newOrder.City = Console.ReadLine();
                            Console.Write("Address: ");
                            newOrder.Address = Console.ReadLine();
                            newOrder.Shipped = false;
                            newOrder.Lines = new List<OrderLine>(); 
                            await _orderRepository.AddOrderAsync(newOrder);
                            break;
                        case "5":
                            Console.Write("Enter ID to update: ");
                            int.TryParse(Console.ReadLine(), out int uid);
                            var update = await _orderRepository.GetOrderAsync(uid);
                            if (update != null)
                            {
                                Console.Write("Customer name: ");
                                update.CustomerName = Console.ReadLine();
                                Console.Write("City: ");
                                update.City = Console.ReadLine();
                                Console.Write("Address: ");
                                update.Address = Console.ReadLine();
                                Console.Write("Shipped (true/false): ");
                                update.Shipped = bool.Parse(Console.ReadLine());
                                await _orderRepository.UpdateOrderAsync(update);
                            }
                            break;
                        case "6":
                            Console.Write("Enter ID to delete: ");
                            int.TryParse(Console.ReadLine(), out int did);
                            var del = await _orderRepository.GetOrderAsync(did);
                            if (del != null)
                                await _orderRepository.DeleteOrderAsync(del);
                            break;
                        case "0": return;
                    }
                    Console.WriteLine("Press Enter to continue...");
                    Console.ReadLine();
                }
            }
        }

        public class ReviewMenu
        {
            private readonly IReview _reviewRepository = new ReviewRepository();

            public async Task ShowAsync()
            {
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("-- Review Menu --");
                    Console.WriteLine("1. View all reviews for book");
                    Console.WriteLine("2. Add review");
                    Console.WriteLine("3. Delete review");
                    Console.WriteLine("0. Back");
                    Console.Write("Select: ");
                    switch (Console.ReadLine())
                    {
                        case "1":
                            Console.Write("Book ID: ");
                            int bookId = int.Parse(Console.ReadLine());
                            var reviews = await _reviewRepository.GetAllReviewsAsync(bookId);
                            foreach (var r in reviews)
                                Console.WriteLine($"User: {r.UserName} | Stars: {r.Stars} | Comment: {r.Comment}");
                            break;
                        case "2":
                            Review review = new();
                            Console.Write("User name: ");
                            review.UserName = Console.ReadLine();
                            Console.Write("Email: ");
                            review.UserEmail = Console.ReadLine();
                            Console.Write("Comment: ");
                            review.Comment = Console.ReadLine();
                            Console.Write("Stars (0-5): ");
                            review.Stars = byte.Parse(Console.ReadLine());
                            Console.Write("Book ID: ");
                            review.BookId = int.Parse(Console.ReadLine());
                            await _reviewRepository.AddReviewAsync(review);
                            break;
                        case "3":
                            Console.Write("Review ID to delete: ");
                            int rid = int.Parse(Console.ReadLine());
                            var reviewToDelete = await _reviewRepository.GetReviewAsync(rid);
                            if (reviewToDelete != null)
                                await _reviewRepository.DeleteReviewAsync(reviewToDelete);
                            break;
                        case "0": return;
                    }
                    Console.WriteLine("Press Enter to continue...");
                    Console.ReadLine();
                }
            }
        }


        internal static ApplicationContext DbContext()
        {
            throw new NotImplementedException();
        }
    }

}

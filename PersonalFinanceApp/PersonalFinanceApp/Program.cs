
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PersonalFinanceApp.Data;
using PersonalFinanceApp.Models;
using PersonalFinanceApp.Services;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false);
var configuration = builder.Build();

var services = new ServiceCollection();
services.AddDbContext<FinanceDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

var provider = services.BuildServiceProvider();
var db = provider.GetRequiredService<FinanceDbContext>();


//Initialize

DbInitializer.Initialize(db);
Console.WriteLine("Database initialized with test data!");

namespace PersonalFinanceApp
{
    internal class Program
    {


        static void Main(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<FinanceDbContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=CompanyDb;Trusted_Connection=True;");

            using var context = new FinanceDbContext(optionsBuilder.Options);

            // Service initialization
            var transactionService = new TransactionService(context);
            var categoryService = new CategoryService(context);
            var reportService = new FinanceReportService(context);

            
            int currentUserId = 1;

            while (true)
            {
                Console.WriteLine("\n===== PERSONAL FINANCE MENU =====");
                Console.WriteLine("1. Add transaction");
                Console.WriteLine("2. Show all transactions");
                Console.WriteLine("3. Income/Expense during a period");
                Console.WriteLine("4. Transaction filtration");
                Console.WriteLine("5. Financical report");
                Console.WriteLine("0. Выход");
                Console.Write("Выберите пункт: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddTransaction();
                        break;
                    case "2":
                        ShowAllTransactions();
                        break;
                    case "3":
                        ShowTotalsForPeriod();
                        break;
                    case "4":
                        FilterTransactions();
                        break;
                    case "5":
                        ShowFinancialReport();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Wrong choice, now die!");
                        break;
                }
            }

            // Methods

            void AddTransaction()
            {
                Console.Write("Enter sum: ");
                decimal amount = decimal.Parse(Console.ReadLine()!);

                Console.Write("Enter descruption: ");
                string description = Console.ReadLine()!;

                Console.Write("Enter date: ");
                DateTime date = DateTime.Parse(Console.ReadLine()!);

                Console.Write("Type (0 - Income, 1 - Expense): ");
                var type = (TransactionType)int.Parse(Console.ReadLine()!);

                var categories = categoryService.GetAllCategories();
                for (int i = 0; i < categories.Count; i++)
                    Console.WriteLine($"{i + 1}. {categories[i].Name}");

                Console.Write("Choose your category: ");
                int categoryIndex = int.Parse(Console.ReadLine()!) - 1;

                transactionService.AddTransaction(
                    currentUserId,
                    amount,
                    description,
                    date,
                    type,
                    categories[categoryIndex].Id
                );

                Console.WriteLine("Transaction added!");
            }

            void ShowAllTransactions()
            {
                var transactions = transactionService.GetAllTransactions(currentUserId);
                Console.WriteLine("\nAll transactions: ");

                foreach (var t in transactions)
                {
                    Console.WriteLine($"{t.Date:yyyy-MM-dd} | {t.Type} | {t.Amount:C} | {t.Category.Name} | {t.Description}");
                }
            }

            void ShowTotalsForPeriod()
            {
                Console.Write("Period beginning: ");
                DateTime start = DateTime.Parse(Console.ReadLine()!);

                Console.Write("Period end: ");
                DateTime end = DateTime.Parse(Console.ReadLine()!);

                var (income, expense) = reportService.GetTotalsForPeriod(currentUserId, start, end);
                Console.WriteLine($"\nIncome: {income:C}");
                Console.WriteLine($" Expenses: {expense:C}");
                Console.WriteLine($"Balance: {(income - expense):C}");
            }

            void FilterTransactions()
            {
                Console.Write("Filter by type (0 - Income, 1 - Expense, empty - everything): ");
                string? typeInput = Console.ReadLine();
                TransactionType? type = string.IsNullOrEmpty(typeInput) ? null : (TransactionType)int.Parse(typeInput);

                Console.Write("Begin date (empty - no filter): ");
                string? startInput = Console.ReadLine();
                DateTime? start = string.IsNullOrEmpty(startInput) ? null : DateTime.Parse(startInput);

                Console.Write("End date: ");
                string? endInput = Console.ReadLine();
                DateTime? end = string.IsNullOrEmpty(endInput) ? null : DateTime.Parse(endInput);

                var results = reportService.FilterTransactions(currentUserId, type, start, end);

                Console.WriteLine("\nTransactions filtered: ");
                foreach (var t in results)
                {
                    Console.WriteLine($"{t.Date:yyyy-MM-dd} | {t.Type} | {t.Amount:C} | {t.Category.Name} | {t.Description}");
                }
            }

            void ShowFinancialReport()
            {
                var report = reportService.GetFullReport(currentUserId);

                Console.WriteLine("\n---FINANCICAL REPORT---");
                Console.WriteLine($"Income: {report.TotalIncome:C}");
                Console.WriteLine($"Expenses: {report.TotalExpense:C}");
                Console.WriteLine($"Balance: {report.Balance:C}");
                Console.WriteLine("\nCategory stats:");

                foreach (var stat in report.CategoryStats)
                {
                    Console.WriteLine($"[{stat.Type}] {stat.Category}: {stat.Total:C}");
                }
            }
        }
    }
}



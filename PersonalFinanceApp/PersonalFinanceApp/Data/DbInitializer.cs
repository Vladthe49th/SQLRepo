using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PersonalFinanceApp.Models;

namespace PersonalFinanceApp.Data
{
    public static class DbInitializer
    {
        public static void Initialize(FinanceDbContext context)
        {
            if (context.Categories.Any() || context.Users.Any())
                return;

            // Categories
            var salary = new TransactionCategory { Name = "Salary" };
            var gifts = new TransactionCategory { Name = "Gifts" };
            var food = new TransactionCategory { Name = "Products" };
            var transport = new TransactionCategory { Name = "Transport" };
            var entertainment = new TransactionCategory { Name = "Entertaiment" };

            var categories = new List<TransactionCategory> { salary, gifts, food, transport, entertainment };
            context.Categories.AddRange(categories);

            // User
            var user = new User
            {
                Name = "Vlad",
                Settings = new UserSettings
                {
                    Currency = "UAH",
                    ShowBalanceInStartup = true
                }
            };

            context.Users.Add(user);
            context.SaveChanges();

            // Transactions
            var transactions = new List<Transaction>
        {
            new()
            {
                Amount = 15000,
                Description = "Monthly fee",
                Date = DateTime.Now.AddDays(-10),
                Type = TransactionType.Income,
                UserId = user.Id,
                CategoryId = salary.Id
            },
            new()
            {
                Amount = 800,
                Description = "Gift from grandma",
                Date = DateTime.Now.AddDays(-7),
                Type = TransactionType.Income,
                UserId = user.Id,
                CategoryId = gifts.Id
            },
            new()
            {
                Amount = 1200,
                Description = "Buying products in supermarket",
                Date = DateTime.Now.AddDays(-5),
                Type = TransactionType.Expense,
                UserId = user.Id,
                CategoryId = food.Id
            },
            new()
            {
                Amount = 300,
                Description = "Metro and marshrutka",
                Date = DateTime.Now.AddDays(-3),
                Type = TransactionType.Expense,
                UserId = user.Id,
                CategoryId = transport.Id
            },
            new()
            {
                Amount = 500,
                Description = "Cinema visit",
                Date = DateTime.Now.AddDays(-2),
                Type = TransactionType.Expense,
                UserId = user.Id,
                CategoryId = entertainment.Id
            }
        };

            context.Transactions.AddRange(transactions);
            context.SaveChanges();
        }
    }


}

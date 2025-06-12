using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PersonalFinanceApp.Data;
using PersonalFinanceApp.Models;

namespace PersonalFinanceApp.Services
{
    public class FinanceReportService
    {
        private readonly FinanceDbContext _context;

        public FinanceReportService(FinanceDbContext context)
        {
            _context = context;
        }

        // Sum of incomes/expenses for a period
        public (decimal income, decimal expense) GetTotalsForPeriod(int userId, DateTime start, DateTime end)
        {
            var transactions = _context.Transactions
                .Where(t => t.UserId == userId && t.Date >= start && t.Date <= end);

            var income = transactions
                .Where(t => t.Type == TransactionType.Income)
                .Sum(t => t.Amount);

            var expense = transactions
                .Where(t => t.Type == TransactionType.Expense)
                .Sum(t => t.Amount);

            return (income, expense);
        }

        // Filtration through type and period
        public List<Transaction> FilterTransactions(int userId, TransactionType? type, DateTime? start, DateTime? end)
        {
            var query = _context.Transactions
                .Where(t => t.UserId == userId);

            if (type.HasValue)
                query = query.Where(t => t.Type == type.Value);

            if (start.HasValue)
                query = query.Where(t => t.Date >= start.Value);

            if (end.HasValue)
                query = query.Where(t => t.Date <= end.Value);

            return query
                .OrderByDescending(t => t.Date)
                .ToList();
        }

        // Full report: incomes, expenses, balance, category stats
        public FinancialReport GetFullReport(int userId)
        {
            var transactions = _context.Transactions
                .Where(t => t.UserId == userId)
                .ToList();

            var income = transactions
                .Where(t => t.Type == TransactionType.Income)
                .Sum(t => t.Amount);

            var expense = transactions
                .Where(t => t.Type == TransactionType.Expense)
                .Sum(t => t.Amount);

            var balance = income - expense;

            var categoryStats = transactions
                .GroupBy(t => t.Category.Name)
                .Select(g => new CategoryStat
                {
                    Category = g.Key,
                    Total = g.Sum(t => t.Amount),
                    Type = g.First().Type
                })
                .ToList();

            return new FinancialReport
            {
                TotalIncome = income,
                TotalExpense = expense,
                Balance = balance,
                CategoryStats = categoryStats
            };
        }
    }
}

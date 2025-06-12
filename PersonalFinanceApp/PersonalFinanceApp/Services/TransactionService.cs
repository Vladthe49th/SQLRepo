using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Data;
using PersonalFinanceApp.Models;

namespace PersonalFinanceApp.Services
{
    public class TransactionService
    {
        private readonly FinanceDbContext _context;

        public TransactionService(FinanceDbContext context)
        {
            _context = context;
        }

        // Add new transaction
        public void AddTransaction(int userId, decimal amount, string description, DateTime date, TransactionType type, int categoryId)
        {
            var transaction = new Transaction
            {
                UserId = userId,
                Amount = amount,
                Description = description,
                Date = date,
                Type = type,
                CategoryId = categoryId
            };

            _context.Transactions.Add(transaction);
            _context.SaveChanges();
        }

        // Get all transactions with categories
        public List<Transaction> GetAllTransactions(int userId)
        {
            return _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.Date)
                .ToList();
        }
    }

}

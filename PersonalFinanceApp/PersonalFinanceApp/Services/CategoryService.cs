using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PersonalFinanceApp.Data;
using PersonalFinanceApp.Models;

namespace PersonalFinanceApp.Services
{
    public class CategoryService
    {
        private readonly FinanceDbContext _context;

        public CategoryService(FinanceDbContext context)
        {
            _context = context;
        }

        public List<TransactionCategory> GetAllCategories()
        {
            return _context.Categories.ToList();
        }
    }
}

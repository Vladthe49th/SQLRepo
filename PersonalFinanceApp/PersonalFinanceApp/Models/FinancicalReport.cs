using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Models
{
    public class FinancialReport
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Balance { get; set; }
        public List<CategoryStat> CategoryStats { get; set; } = new();
    }

    public class CategoryStat
    {
        public string Category { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public TransactionType Type { get; set; }
    }
}

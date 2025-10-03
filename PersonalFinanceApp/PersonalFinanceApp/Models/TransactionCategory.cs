using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace PersonalFinanceApp.Models
{
    public class TransactionCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public List<Transaction> Transactions { get; set; } = new();
    }

}

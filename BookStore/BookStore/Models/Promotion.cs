using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class Promotion
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal? Percent {  get; set; }
        public decimal? Amount { get; set; }
        public int BookId { get; set; }
        public Book? Book { get; set; }

        public override string ToString()
        {
            return $"Name - {Name}\n" +
           (Percent.HasValue ? $"Discount - {Percent}%"
                      : $"Discount - {Amount} uah");

        }
    }
}



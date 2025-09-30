using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public bool Shipped { get; set; }

        public virtual ICollection<OrderLine> Lines { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string UserName { get; set; }

        public string UserEmail { get; set; }
        public string Comment { get; set; }
        public byte Stars { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public DateTime PublishedOn { get; set; }

        public string? Publisher { get; set; }

        public decimal Price { get; set; }

        public int? PromotionId { get; set; }


        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public Promotion? Promotion { get; set; }

        public virtual ICollection<Author> Authors { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }

    }
}



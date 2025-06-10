using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BookStore.Models;

namespace BookStore.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        
        public DbSet<Author> Authors { get; set; }

        public DbSet<Review> Reviews { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Promotion> Promotions { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) 
            : base(options)
        { 
        
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
                .HasMany<Author>(s => s.Authors)
                .WithMany(c => c.Books)
                .UsingEntity(e => e.ToTable("BookAuthor"));
        }

        internal object Where(Func<object, bool> value)
        {
            throw new NotImplementedException();
        }
    }
}

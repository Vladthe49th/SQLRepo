using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;

namespace Entity_class6
{
    // Book class
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int AuthorId { get; set; }
        public int GenreId { get; set; }
        public decimal Price { get; set; }

        public Author Author { get; set; }
        public Genre Genre { get; set; }
    }

    // Author class
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Book> Books { get; set; }
    }

    // Genre class
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Book> Books { get; set; }
    }


    // Context
    public class BookContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genre> Genres { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=DishMenuDb;Trusted_Connection=True;"); // Замени на свою строку подключения
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId);

            modelBuilder.Entity<Book>()
                .HasOne(b => b.Genre)
                .WithMany(g => g.Books)
                .HasForeignKey(b => b.GenreId);
        }
    }




    internal class Program
    {
        static void Main(string[] args)
        {
            using (var context = new BookContext())
            {
                if (!context.Authors.Any())
                {
                    context.Authors.AddRange(
                        new Author { Id = 1, Name = "Fat" },
                        new Author { Id = 2, Name = "Dostoevski" }
                    );
                }

                if (!context.Genres.Any())
                {
                    context.Genres.AddRange(
                        new Genre { Id = 1, Name = "Romance" },
                        new Genre { Id = 2, Name = "Sci-fi" }
                    );
                }

                if (!context.Books.Any())
                {
                    context.Books.AddRange(
                        new Book { Title = "War and peace", AuthorId = 1, GenreId = 1, Price = 500 },
                        new Book { Title = "Anna Karenina", AuthorId = 1, GenreId = 1, Price = 400 },
                        new Book { Title = "Idiot", AuthorId = 2, GenreId = 1, Price = 350 },
                        new Book { Title = "Karamazov bros", AuthorId = 2, GenreId = 1, Price = 600 }
                    );
                }

                context.SaveChanges();
            }


            // 1) Num of certain genre books

            using (var context = new BookContext())
            {
                int genreId = 1;
                var count = context.Books.Count(b => b.GenreId == genreId);

                // 2) Minimum price of an author`s book
                int authorId = 1;
                var minPrice = context.Books
                    .Where(b => b.AuthorId == authorId)
                    .Min(b => b.Price);

                // 3) Average price for certain genre books
                var avgPrice = context.Books
                    .Where(b => b.GenreId == genreId)
                    .Average(b => b.Price);

                // 4) Sum of an author`s book prices
                var totalPrice = context.Books
                    .Where(b => b.AuthorId == authorId)
                    .Sum(b => b.Price);

                // 5) Group by genre
                var groupedByGenre = context.Books
                    .GroupBy(b => b.Genre.Name)
                    .ToList();

                // 6) Genre book names
                var titles = context.Books
                    .Where(b => b.GenreId == genreId)
                    .Select(b => b.Title)
                    .ToList();

                // 7) All books except for certain genre
                var booksToExclude = context.Books.Where(b => b.GenreId == genreId);
                var otherBooks = context.Books.Except(booksToExclude).ToList();

                // 8) Books of two authors
                int author2Id = 2;
                var booksFromTwoAuthors = context.Books
                    .Where(b => b.AuthorId == authorId)
                    .Union(context.Books.Where(b => b.AuthorId == author2Id))
                    .ToList();

                // 9) Top 5 expensive books
                var topExpensiveBooks = context.Books
                    .OrderByDescending(b => b.Price)
                    .Take(5)
                    .ToList();

                // 10) Skip 10, take next 5
                var pagedBooks = context.Books
                    .OrderBy(b => b.Id)
                    .Skip(10)
                    .Take(5)
                    .ToList();


            }

        }
    }
}

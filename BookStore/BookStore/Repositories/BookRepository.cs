using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookStore.Data;
using BookStore.Interfaces;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;


namespace BookStore.Repositories
{
    public class BookRepository: IBook
    {

        public async Task AddBookAsync(Book book)
        {
            using (ApplicationContext context = Program.DbContext())
            {
                context.Authors.AttachRange(book.Authors);
                await context.Books.AddAsync(book);
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteBookAsync(Book book)
        {
            using (ApplicationContext context = Program.DbContext())
            {
                context.Remove(book);
                await context.SaveChangesAsync();
            }

        }

        public async Task EditBookAsync(Book book)
        {
            using (ApplicationContext context = Program.DbContext())
            {
                var currentBook = context.Books
                    .Include(b => b.Authors)
                    .FirstOrDefault(b => b.Id == book.Id);

                if (currentBook is null) return;

                context.Entry(currentBook).CurrentValues.SetValues(book);

                var incomingAuthorIds = book.Authors.Select(a => a.Id).ToList();

                var updatedAuthors = context.Authors
                    .Where(a =>  incomingAuthorIds.Contains(a.Id))
                    .ToList();

                currentBook.Authors = currentBook.Authors.Where(a => !incomingAuthorIds.Contains(a.Id)).ToList();

                foreach (var author in updatedAuthors)
                {
                    if (!currentBook.Authors.Any(a => a.Id == author.Id))
                    {
                        currentBook.Authors.Add(author);
                    }
                }

                context.SaveChanges();
                
                    
                
            } 


        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            using (ApplicationContext context = Program.DbContext())
            {
                return await context.Books.ToListAsync();
            }

        }
      

        public async Task<IEnumerable<Book>> GetAllBooksWithAuthorsAsync()
        {
            using (ApplicationContext context = Program.DbContext())
            {
                return await context.Books.Include(e => e.Authors).ToListAsync();
            }
        }

        public async Task<Book> GetBookAsync(int id)
        {
            using (ApplicationContext context = Program.DbContext())
            {
                return await context.Books.FirstOrDefaultAsync(e => e.Id == id);
            }
        }

        public async Task<IEnumerable<Book>> GetBooksByNameAsync(string name)
        {
            using (ApplicationContext context = Program.DbContext())
            {
               return await context.Books.Where(e => e.Name.Contains(name)).ToListAsync();
            }
        }

        public async Task<Book> GetBookWithAuthorsAndReviewAsync(int id)
        {
            using (ApplicationContext context = Program.DbContext())
            {
                return await context.Books.Include(e => e.Authors).Include(e => e.Reviews).FirstOrDefaultAsync(e => e.Id == id);
            }

        }

        public async Task<Book> GetBookWithAuthorsAsync(int id)
        {
            using (ApplicationContext context = Program.DbContext())
            {
                return await context.Books.Include(e => e.Authors).FirstOrDefaultAsync(e => e.Id == id);
            }
        }

        public async Task<Book> GetBookWithCategoryAndAuthorsAsync(int id)
        {
            using (ApplicationContext context = Program.DbContext())
            {
                return await context.Books.Include(e => e.Category).Include(e => e.Authors).FirstOrDefaultAsync( e => e.Id == id);
            }
        }

        public async Task<Book> GetBookWithPromotionAsync(int id)
        {
            using (ApplicationContext context = Program.DbContext())
            {
                return await context.Books.Include(e => e.Promotion).FirstOrDefaultAsync(e => e.Id == id);
            }
        }
     

     
        public async Task<Book> GetBookWithAuthorsAndReviewAndCategoryAsync(int id)
        {
            using (ApplicationContext context = Program.DbContext())
            {
                return await context.Books.Include(e => e.Category).Include(e => e.Authors).Include(e => e.Reviews).FirstOrDefaultAsync(e => e.Id == id);
            }
        }
     
        

      

        
    }
}

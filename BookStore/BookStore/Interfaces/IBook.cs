using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookStore.Models;

namespace BookStore.Interfaces
{
    public interface IBook
    {
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<IEnumerable<Book>> GetAllBooksWithAuthorsAsync();

        Task<Book> GetBookAsync(int id);
        Task<IEnumerable<Book>> GetBooksByNameAsync(string name);
        Task<Book> GetBookWithPromotionAsync(int id);
        Task<Book> GetBookWithAuthorsAsync(int id);
        Task<Book> GetBookWithCategoryAndAuthorsAsync( int id);
        Task<Book> GetBookWithAuthorsAndReviewAsync(int id);
        Task<Book> GetBookWithAuthorsAndReviewAndCategoryAsync( int id);

        Task AddBookAsync(Book book);

        Task DeleteBookAsync(Book book);

        Task EditBookAsync(Book book);


    }
}

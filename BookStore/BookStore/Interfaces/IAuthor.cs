using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookStore.Models;

namespace BookStore.Interfaces
{
    public interface IAuthor
    {
        Task<IEnumerable<Author>> GetAllAuthorsAsync();
        Task<Author> GetAuthorWithBooksAsync(int id);
        Task<Author> GetAuthorAsync(int id);

        Task<IEnumerable<Author>> GetAuthorsByName(string name);

        Task AddAuthorAsync(Author author);
        Task DeleteAuthorAsync(Author author);
        Task EditAuthorAsync(Author author);
    }
}

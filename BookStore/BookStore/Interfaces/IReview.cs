using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookStore.Models;

namespace BookStore.Interfaces
{
    public interface IReview
    {
        Task<IEnumerable<Review>> GetAllReviewsAsync(int bookId);
        Task<Review> GetReviewAsync(int id);

        Task AddReviewAsync(Review review);
        Task DeleteReviewAsync(Review review);
    }
}

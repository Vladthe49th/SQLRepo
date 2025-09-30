using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BookStore.Data;
using BookStore.Interfaces;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Repositories
{
    public class ReviewRepository : IReview
    {

        public async Task AddReviewAsync(Review review)
        {
            using (ApplicationContext context = Program.DbContext())
            {
                await context.Reviews.AddAsync(review);
                await context.SaveChangesAsync();
            }

        }

        public async Task DeleteReviewAsync(Review review)
        {
            using (ApplicationContext context = Program.DbContext())
            {
                context.Reviews.Remove(review);
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Review>> GetAllReviewsAsync(int bookId)
        {
            using (ApplicationContext context = Program.DbContext())
            {
                return await context.Reviews.Where(e => e.BookId == bookId).ToListAsync();
            }

        }

        public async Task<Review> GetReviewAsync(int id)
        {
            using (ApplicationContext context = Program.DbContext())
            {
                return await context.Reviews.FirstOrDefaultAsync(e => e.Id == id);
            }
        }



    }
}

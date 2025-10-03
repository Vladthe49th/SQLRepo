using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookStore.Data;
using BookStore.Interfaces;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Repositories
{
    public class CategoryRepository : ICategory


    {
        public async Task AddCategoryAsync(Category category)
        {
            using (ApplicationContext context = Program.DbContext())
            {
                await context.Categories.AddAsync(category);
                await context.SaveChangesAsync();
            }

        }

        public async Task DeleteCategoryAsync(Category category)
        {
            using (ApplicationContext context = Program.DbContext())
            {
                context.Categories.Remove(category);
                await context.SaveChangesAsync();
            }

        }

        public async Task UpdateCategoryAsync(Category category)
        {
            using (ApplicationContext context = Program.DbContext())
            {
                context.Categories.Update(category);
                await context.SaveChangesAsync();
            }
        }



        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            using (ApplicationContext context = Program.DbContext())
            {
                return await context.Categories.ToListAsync();
            }

        }




       public async Task<IEnumerable<Category>> GetCategoriesByNameAsync(string name)
        {
             using (ApplicationContext context = Program.DbContext())
            {
                return await context.Categories.Where(e => e.Name.Contains(name)).ToListAsync();
            }
        }


        public async Task<Category> GetCategoryAsync(int id)
        {

            using (ApplicationContext context = Program.DbContext())
            {
                return await context.Categories.FirstOrDefaultAsync(e => e.Id == id);
            }
        }


        public async Task<Category> GetCategoryWithBooksAsync(int id)
        {
            using (ApplicationContext context = Program.DbContext())
            {
                return await context.Categories.Include(e => e.Books).FirstOrDefaultAsync(e => e.Id == id);
            }
        }


      
        

       
    }
}

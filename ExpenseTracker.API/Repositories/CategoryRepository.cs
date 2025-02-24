using ExpenseTracker.API.Database;
using ExpenseTracker.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.API.Repositories
{
    internal sealed class CategoryRepository
        (
            ExpenseContext context
        )
        : ICategoryRepository
    {
        public async Task<Category> Add(Category category)
        {
            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> ContainExpenses(int id)
        {
            return await context.Expenses.AnyAsync(e => e.CategoryId == id);
        }

        public async Task<bool> Delete(Category category)
        {
            context.Categories.Remove(category);
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Category>> GetAll()
        {
            return await context.Categories.ToListAsync();
        }

        public async Task<Category?> GetById(int id)
        {
            return await context.Categories
                                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<decimal> GetCategoriesTotalExpenses(int id)
        {
            return await context.Expenses.Where(e => e.CategoryId == id)
                                         .SumAsync(e => e.Amount);
        }

        public async Task<Category> Update(Category category)
        {
            context.Categories.Update(category);
            await context.SaveChangesAsync();
            return category;
        }
    }
}

using ExpenseTracker.API.Models;

namespace ExpenseTracker.API.Repositories
{
    public interface ICategoryRepository
    {
        Task<Category?> GetById(int id);
        Task<Category> Add(Category category);
        Task<Category> Update(Category category);
        Task<bool> Delete(Category category);
        Task<IEnumerable<Category>> GetAll();
        Task<bool> ContainExpenses(int id);
    }
}

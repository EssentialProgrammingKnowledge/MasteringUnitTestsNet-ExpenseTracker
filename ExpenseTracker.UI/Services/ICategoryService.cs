using ExpenseTracker.UI.Models;

namespace ExpenseTracker.UI.Services
{
    public interface ICategoryService
    {
        Task<Result> Add(CategoryDTO dto);
        Task<Result> Update(CategoryDTO dto);
        Task<Result> Delete(int id);
        Task<Result<CategoryDTO?>> GetById(int id);
        Task<Result<List<CategoryDTO>>> GetAll();
    }
}

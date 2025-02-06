using ExpenseTracker.UI.Models;

namespace ExpenseTracker.UI.Services
{
    public interface ICategoryService
    {
        Task Add(CategoryDTO dto);
        Task Update(CategoryDTO dto);
        Task Delete(int id);
        Task<CategoryDTO?> GetById(int id);
        Task<List<CategoryDTO>> GetAll();
    }
}

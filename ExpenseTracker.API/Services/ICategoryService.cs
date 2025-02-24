using ExpenseTracker.API.DTO;

namespace ExpenseTracker.API.Services
{
    public interface ICategoryService
    {
        Task<Result<CategoryDTO>> AddCategory(CategoryDTO categoryDto);
        Task<Result<CategoryDTO>> UpdateCategory(CategoryDTO categoryDto);
        Task<Result> DeleteCategory(int id);
        Task<Result<CategoryDTO>> GetCategoryById(int id);
        Task<IEnumerable<CategoryDTO>> GetAllCategories();
    }
}

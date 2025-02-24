using ExpenseTracker.API.DTO;
using ExpenseTracker.API.Models;
using ExpenseTracker.API.Repositories;

namespace ExpenseTracker.API.Services
{
    internal sealed class CategoryService
        (
            ICategoryRepository categoryRepository
        )
        : ICategoryService
    {
        public async Task<Result<CategoryDTO>> AddCategory(CategoryDTO categoryDto)
        {
            await categoryRepository.Add(new Category());
            return Result<CategoryDTO>.CreatedResult(categoryDto);
        }

        public async Task<Result> DeleteCategory(int id)
        {
            await categoryRepository.Delete(new Category());
            return Result.NoContentResult();
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllCategories()
        {
            await Task.CompletedTask;
            return [];
        }

        public async Task<Result<CategoryDTO>> GetCategoryById(int id)
        {
            await categoryRepository.GetById(id);
            return Result<CategoryDTO>.OkResult(new CategoryDTO(id, "", 0));
        }

        public async Task<Result<CategoryDTO>> UpdateCategory(CategoryDTO categoryDto)
        {
            await categoryRepository.Update(new Category());
            return Result<CategoryDTO>.OkResult(categoryDto);
        }
    }
}

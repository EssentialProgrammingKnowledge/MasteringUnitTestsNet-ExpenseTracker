using ExpenseTracker.API.DTO;
using ExpenseTracker.API.Mappings;
using ExpenseTracker.API.Models;
using ExpenseTracker.API.Repositories;
using ExpenseTracker.API.Validations;

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
            var result = Validate(categoryDto);
            if (!result.Success)
            {
                return Result<CategoryDTO>.BadRequestResult(result.ErrorMessage!);
            }

            var category = new Category
            {
                Name = categoryDto.Name,
                Budget = categoryDto.Budget
            };
            await categoryRepository.Add(category);
            return Result<CategoryDTO>.CreatedResult(category.AsDto());
        }

        public async Task<Result> DeleteCategory(int id)
        {
            var category = await categoryRepository.GetById(id);
            if (category is null)
            {
                return Result.NotFoundResult(CategoryErrorMessages.NotFound(id));
            }

            if (await categoryRepository.ContainExpenses(id))
            {
                return Result.BadRequestResult(CategoryErrorMessages.CannotDeleteCategoryWithExpenses());
            }

            var result = await categoryRepository.Delete(category);
            return result ?
                Result.NoContentResult()
                : Result.NotFoundResult(CategoryErrorMessages.NotFound(id));
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllCategories()
        {
            return (await categoryRepository.GetAll())
                                            .Select(c => c.AsDto());
        }

        public async Task<Result<CategoryDTO>> GetCategoryById(int id)
        {
            var category = await categoryRepository.GetById(id);
            if (category is null)
            {
                return Result<CategoryDTO>.NotFoundResult(CategoryErrorMessages.NotFound(id));
            }

            return Result<CategoryDTO>.OkResult(category.AsDto());
        }

        public async Task<Result<CategoryDTO>> UpdateCategory(CategoryDTO categoryDto)
        {
            var result = Validate(categoryDto);
            if (!result.Success)
            {
                return Result<CategoryDTO>.BadRequestResult(result.ErrorMessage!);
            }

            var category = await categoryRepository.GetById(categoryDto.Id);
            if (category is null)
            {
                return Result<CategoryDTO>.NotFoundResult(CategoryErrorMessages.NotFound(categoryDto.Id));
            }

            var totalExpensesCost = await categoryRepository.GetCategoriesTotalExpenses(categoryDto.Id);
            if (totalExpensesCost > categoryDto.Budget)
            {
                return Result<CategoryDTO>.BadRequestResult(CategoryErrorMessages.LowerBudgetThanTotalExpenses(categoryDto.Budget, totalExpensesCost));
            }

            category.Name = categoryDto.Name;
            category.Budget = categoryDto.Budget;
            await categoryRepository.Update(category);
            return Result<CategoryDTO>.OkResult(category.AsDto());
        }

        private ValidationResult Validate(CategoryDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return ValidationResult.FailureResult(CategoryErrorMessages.NameCannotBeEmpty());
            }

            if (dto.Name.Length > 100)
            {
                return ValidationResult.FailureResult(CategoryErrorMessages.NameTooLong(100, dto.Name.Length));
            }

            if (dto.Budget <= 0)
            {
                return ValidationResult.FailureResult(CategoryErrorMessages.BudgetMustBeGreaterThanZero());
            }

            return ValidationResult.SuccessResult();
        }
    }
}

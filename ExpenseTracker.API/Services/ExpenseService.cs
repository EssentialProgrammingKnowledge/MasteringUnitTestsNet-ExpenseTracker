using ExpenseTracker.API.DTO;
using ExpenseTracker.API.Mappings;
using ExpenseTracker.API.Models;
using ExpenseTracker.API.Repositories;
using ExpenseTracker.API.Validations;

namespace ExpenseTracker.API.Services
{
    internal sealed class ExpenseService
        (
            IExpenseRepository expenseRepository, 
            ICategoryRepository categoryRepository
        ) : IExpenseService
    {
        public async Task<Result<ExpenseDetailsDTO>> AddExpense(ExpenseDTO expenseDto)
        {
            var result = ValidateExpense(expenseDto);
            if (!result.Success)
            {
                return Result<ExpenseDetailsDTO>.BadRequestResult(result.ErrorMessage!);
            }

            var category = await categoryRepository.GetById(expenseDto.CategoryId);
            if (category is null)
            {
                return Result<ExpenseDetailsDTO>.BadRequestResult(CategoryErrorMessages.NotFound(expenseDto.CategoryId));
            }

            var budgetExceededResult = await ShouldNotExceedTheBudget(category, expenseDto);
            if (!budgetExceededResult.Success)
            {
                return Result<ExpenseDetailsDTO>.BadRequestResult(budgetExceededResult.ErrorMessage!);
            }

            var expense = new Expense
            {
                Amount = expenseDto.Amount,
                CategoryId = expenseDto.CategoryId,
                Description = expenseDto.Description,
                Category = category
            };
            expense = await expenseRepository.Add(expense);
            return Result<ExpenseDetailsDTO>.CreatedResult(expense.AsDetailsDto());
        }

        public async Task<Result> DeleteExpense(int id)
        {
            var result = await expenseRepository.Delete(id);
            return result ?
                Result.NoContentResult()
                : Result.NotFoundResult(ExpenseErrorMessages.NotFound(id));
        }

        public async Task<IEnumerable<ExpenseDTO>> GetAllExpenses()
        {
            return (await expenseRepository.GetAll())
                                           .Select(e => e.AsDto())
                                           .ToList();
        }

        public async Task<Result<ExpenseDetailsDTO>> GetExpenseById(int id)
        {
            var expense = await expenseRepository.GetById(id);
            if (expense is null)
            {
                return Result<ExpenseDetailsDTO>.NotFoundResult(ExpenseErrorMessages.NotFound(id));
            }

            return Result<ExpenseDetailsDTO>.OkResult(expense.AsDetailsDto());
        }

        public async Task<Result<ExpenseDetailsDTO>> UpdateExpense(ExpenseDTO expenseDto)
        {
            var result = ValidateExpense(expenseDto);
            if (!result.Success)
            {
                return Result<ExpenseDetailsDTO>.BadRequestResult(result.ErrorMessage!);
            }

            var expense = await expenseRepository.GetById(expenseDto.Id);
            if (expense is null)
            {
                return Result<ExpenseDetailsDTO>.NotFoundResult(ExpenseErrorMessages.NotFound(expenseDto.Id));
            }

            var category = await categoryRepository.GetById(expenseDto.CategoryId);
            if (category is null)
            {
                return Result<ExpenseDetailsDTO>.BadRequestResult(CategoryErrorMessages.NotFound(expenseDto.CategoryId));
            }

            var budgetExceededResult = await ShouldNotExceedTheBudget(category, expenseDto, expense);
            if (!budgetExceededResult.Success)
            {
                return Result<ExpenseDetailsDTO>.BadRequestResult(budgetExceededResult.ErrorMessage!);
            }

            expense.Description = expenseDto.Description;
            expense.Amount = expenseDto.Amount;
            expense.CategoryId = expenseDto.CategoryId;
            await expenseRepository.Update(expense);
            return Result<ExpenseDetailsDTO>.OkResult(expense.AsDetailsDto());
        }

        private ValidationResult ValidateExpense(ExpenseDTO expenseDto)
        {
            if (expenseDto.Amount <= 0)
            {
                return ValidationResult.FailureResult(ExpenseErrorMessages.AmountMustBeGreaterThanZero());
            }

            if (string.IsNullOrWhiteSpace(expenseDto.Description))
            {
                return ValidationResult.FailureResult(ExpenseErrorMessages.DescriptionCannotBeEmpty());
            }

            if (expenseDto.Description.Length > 250)
            {
                return ValidationResult.FailureResult(ExpenseErrorMessages.DescriptionTooLong(250, expenseDto.Description.Length));
            }

            return ValidationResult.SuccessResult();
        }

        private async Task<ValidationResult> ShouldNotExceedTheBudget(Category category, ExpenseDTO expenseDto, Expense? expense = null)
        {
            decimal totalExpenses = await expenseRepository.GetTotalExpensesAmount(expenseDto.CategoryId);
            decimal newTotalExpenses = totalExpenses - (expense?.Amount ?? 0) + expenseDto.Amount;

            if (newTotalExpenses > category.Budget)
            {
                return ValidationResult.FailureResult(ExpenseErrorMessages.AmountExceedsBudget(expenseDto.Amount, category.Budget, newTotalExpenses));
            }
            return ValidationResult.SuccessResult();
        }
    }
}

using ExpenseTracker.API.DTO;
using ExpenseTracker.API.Models;

namespace ExpenseTracker.API.Mappings
{
    public static class ExpenseExtensions
    {
        public static ExpenseDTO AsDto(this Expense expense)
        {
            return new ExpenseDTO(expense.Id, expense.Amount, expense.CategoryId, expense.Description);
        }

        public static ExpenseDetailsDTO AsDetailsDto(this Expense expense)
        {
            return new ExpenseDetailsDTO(
                expense.Id,
                expense.Amount,
                expense.Description,
                expense.Category is null ?
                    new CategoryDTO(expense.CategoryId, string.Empty, decimal.Zero)
                    : new CategoryDTO(expense.Category.Id, expense.Category.Name, expense.Category.Budget)
            );
        }
    }
}

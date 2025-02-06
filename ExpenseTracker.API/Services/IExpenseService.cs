using ExpenseTracker.API.DTO;

namespace ExpenseTracker.API.Services
{
    public interface IExpenseService
    {
        Task<Result<ExpenseDetailsDTO>> AddExpense(ExpenseDTO expenseDto);
        Task<Result<ExpenseDetailsDTO>> UpdateExpense(ExpenseDTO expenseDto);
        Task<Result> DeleteExpense(int id);
        Task<Result<ExpenseDetailsDTO>> GetExpenseById(int id);
        Task<IEnumerable<ExpenseDTO>> GetAllExpenses();
    }
}

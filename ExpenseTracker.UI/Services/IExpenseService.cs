using ExpenseTracker.UI.Models;

namespace ExpenseTracker.UI.Services
{
    public interface IExpenseService
    {
        Task<Result> Add(ExpenseDTO dto);
        Task<Result> Update(ExpenseDTO dto);
        Task<Result> Delete(int id);
        Task<Result<ExpenseDetailsDTO?>> GetById(int id);
        Task<Result<List<ExpenseDTO>>> GetAll();
    }
}

using ExpenseTracker.API.Models;

namespace ExpenseTracker.API.Repositories
{
    public interface IExpenseRepository
    {
        Task<Expense?> GetById(int id);
        Task<Expense> Add(Expense expense);
        Task<Expense> Update(Expense expense);
        Task<bool> Delete(int id);
        Task<IEnumerable<Expense>> GetAll();
        Task<decimal> GetTotalExpensesAmount(int categoryId);
    }
}

using ExpenseTracker.API.Database;
using ExpenseTracker.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.API.Repositories
{
    internal sealed class ExpenseRepository
        (
            ExpenseContext context
        )
        : IExpenseRepository
    {
        public async Task<Expense> Add(Expense expense)
        {
            await context.Expenses.AddAsync(expense);
            await context.SaveChangesAsync();
            return expense;
        }

        public async Task<bool> Delete(int id)
        {
            var expense = await context.Expenses
                                       .FirstOrDefaultAsync(e => e.Id == id);
            if (expense is null)
            {
                return false;
            }

            context.Expenses.Remove(expense);
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Expense>> GetAll()
        {
            return await context.Expenses.ToListAsync();
        }

        public async Task<Expense?> GetById(int id)
        {
            return await context.Expenses
                                .Include(e => e.Category)
                                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<decimal> GetTotalExpensesAmount(int categoryId)
        {
            return await context.Expenses.Where(e => e.CategoryId == categoryId).SumAsync(e => e.Amount);
        }

        public async Task<Expense> Update(Expense expense)
        {
            context.Expenses.Update(expense);
            await context.SaveChangesAsync();
            return expense;
        }
    }
}

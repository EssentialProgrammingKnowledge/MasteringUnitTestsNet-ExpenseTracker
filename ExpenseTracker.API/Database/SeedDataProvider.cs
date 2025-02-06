using ExpenseTracker.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.API.Database
{
    public class SeedDataProvider
        (
            ExpenseContext expenseContext
        ) : ISeedDataProvider
    {
        public async Task SeedData(CancellationToken cancellationToken = default)
        {
            if (await expenseContext.Categories.AnyAsync(cancellationToken))
            {
                return;
            }

            var homeExpenseCategory = new Category
            {
                Name = "Domowe wydatki",
                Budget = 15000,
                Expenses = [
                    new Expense
                    {
                        Description = "Laptop Dell",
                        Amount = 6500
                    },
                    new Expense
                    {
                        Description = "Zmywarka Bosch",
                        Amount = 2500
                    },
                    new Expense
                    {
                        Description = "Pralka",
                        Amount = 3000
                    }
                ]
            };
            var roadExpenseCategory = new Category
            {
                Name = "Wydatki na drogę",
                Budget = 1000,
                Expenses = [
                    new Expense
                    {
                        Description = "Paliwo Orlen",
                        Amount = 500
                    },
                    new Expense
                    {
                        Description = "Paliwo BP",
                        Amount = 250
                    },
                    new Expense
                    {
                        Description = "Opłata za bramki",
                        Amount = 50
                    }
                ]
            };
            var gameExpenseCategory = new Category
            {
                Name = "Budżet na grę",
                Budget = 100,
                Expenses = [
                    new Expense
                    {
                        Description = "Skórki",
                        Amount = 50
                    }
                ]
            };

            await expenseContext.Categories.AddRangeAsync([
                homeExpenseCategory,
                roadExpenseCategory,
                gameExpenseCategory
            ], cancellationToken);
            await expenseContext.SaveChangesAsync(cancellationToken);
        }
    }
}

using ExpenseTracker.API.DTO;
using ExpenseTracker.API.Mappings;
using ExpenseTracker.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpensesController
        (
            IExpenseService expenseService
        )
        : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<ExpenseDTO>> GetExpenses()
        {
            return await expenseService.GetAllExpenses();
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<ExpenseDetailsDTO>> GetExpense(int id)
        {
            return (await expenseService.GetExpenseById(id)).ToActionResult();
        }

        [HttpPost]
        public async Task<ActionResult<ExpenseDetailsDTO>> AddExpense(ExpenseDTO expense)
        {
            var result = await expenseService.AddExpense(expense);
            return result.ToCreatedActionResult(this, nameof(GetExpense), new { result.Data?.Id });
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ExpenseDetailsDTO>> UpdateExpense(int id, ExpenseDTO expense)
        {
            return (await expenseService.UpdateExpense(expense with { Id = id }))
                .ToActionResult();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteExpense(int id)
        {
            return (await expenseService.DeleteExpense(id))
                .ToActionResult();
        }
    }
}

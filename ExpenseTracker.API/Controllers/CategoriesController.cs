using ExpenseTracker.API.Database;
using ExpenseTracker.API.DTO;
using ExpenseTracker.API.Mappings;
using ExpenseTracker.API.Models;
using ExpenseTracker.API.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController
        (
            ExpenseContext context
        ) : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<CategoryDTO>> GetCategories()
        {
            var categories = await context.Categories
                .Select(c => c.AsDto())
                .ToListAsync();
            return categories;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDTO>> GetCategory(int id)
        {
            var category = await context.Categories.FindAsync(id);

            if (category is null)
            {
                return NotFound(CategoryErrorMessages.NotFound(id));
            }

            return Ok(category.AsDto());
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDTO>> CreateCategory(CategoryDTO dto)
        {
            var category = new Category
            {
                Name = dto.Name,
                Budget = dto.Budget,
            };
            context.Categories.Add(category);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category.AsDto());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CategoryDTO>> UpdateCategory(int id, CategoryDTO dto)
        {
            var category = await context.Categories
                .Include(c => c.Expenses)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (category is null)
            {
                return NotFound(CategoryErrorMessages.NotFound(id));
            }
            decimal totalExpenses = category.Expenses.Sum(e => e.Amount);
            if (dto.Budget < totalExpenses)
            {
                return BadRequest(CategoryErrorMessages.LowerBudgetThanTotalExpenses(dto.Budget, totalExpenses));
            }

            category.Name = dto.Name;
            category.Budget = dto.Budget;
            context.Categories.Update(category);
            await context.SaveChangesAsync();

            return Ok(category.AsDto());
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            var category = await context.Categories
                .Include(c => c.Expenses)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (category is null)
            {
                return NotFound(CategoryErrorMessages.NotFound(id));
            }

            if (await context.Expenses.AnyAsync(e => e.CategoryId == id))
            {
                return BadRequest(CategoryErrorMessages.CannotDeleteCategoryWithExpenses());
            }

            context.Categories.Remove(category);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}

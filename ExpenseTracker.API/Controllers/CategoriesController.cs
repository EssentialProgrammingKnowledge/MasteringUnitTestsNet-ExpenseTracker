using ExpenseTracker.API.DTO;
using ExpenseTracker.API.Mappings;
using ExpenseTracker.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController
        (
            ICategoryService categoryService
        ) : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<CategoryDTO>> GetCategories()
        {
            return await categoryService.GetAllCategories();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDTO>> GetCategory(int id)
        {
            return (await categoryService.GetCategoryById(id))
                            .ToActionResult();
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDTO>> CreateCategory(CategoryDTO dto)
        {
            return (await categoryService.AddCategory(dto))
                            .ToCreatedActionResult(this, nameof(GetCategory), new { dto.Id });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CategoryDTO>> UpdateCategory(int id, CategoryDTO dto)
        {
            return (await categoryService.UpdateCategory(dto with { Id = id }))
                            .ToActionResult();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            return (await categoryService.DeleteCategory(id))
                            .ToActionResult();
        }
    }
}

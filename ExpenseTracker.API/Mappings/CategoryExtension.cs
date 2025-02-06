using ExpenseTracker.API.DTO;
using ExpenseTracker.API.Models;

namespace ExpenseTracker.API.Mappings
{
    public static class CategoryExtension
    {
        public static CategoryDTO AsDto(this Category category)
        {
            return new CategoryDTO(category.Id, category.Name, category.Budget);
        }
    }
}

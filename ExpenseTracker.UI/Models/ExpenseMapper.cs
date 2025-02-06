namespace ExpenseTracker.UI.Models
{
    public static class ExpenseMapper
    {
        public static ExpenseDTO? ToDto(this ExpenseDetailsDTO dto)
        {
            if (dto is null)
            {
                return null;
            }

            return new ExpenseDTO
            {
                Id = dto.Id,
                Description = dto.Description,
                Amount = dto.Amount,
                CategoryId = dto.Category.Id
            };
        }
    }
}

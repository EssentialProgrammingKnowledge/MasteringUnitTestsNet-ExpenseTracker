namespace ExpenseTracker.API.DTO
{
    public record ExpenseDetailsDTO(int Id, decimal Amount, string Description, CategoryDTO Category);
}

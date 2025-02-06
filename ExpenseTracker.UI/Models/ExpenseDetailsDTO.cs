namespace ExpenseTracker.UI.Models
{
    public record ExpenseDetailsDTO
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public CategoryDTO Category { get; set; } = null!;
    }
}

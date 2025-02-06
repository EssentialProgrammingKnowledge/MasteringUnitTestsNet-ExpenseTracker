using FluentValidation;

namespace ExpenseTracker.UI.Models
{
    public record CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Budget { get; set; }
    }

    public class CategoryValidator : AbstractValidator<CategoryDTO>
    {
        public CategoryValidator()
        {
            RuleFor(e => e.Name)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(100);

            RuleFor(e => e.Budget)
                .GreaterThan(0);
        }
    }
}

using FluentValidation;

namespace ExpenseTracker.UI.Models
{
    public record ExpenseDTO
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int CategoryId { get; set; }
    }

    public class ExpenseValidator : AbstractValidator<ExpenseDTO>
    {
        public ExpenseValidator()
        {
            RuleFor(e => e.Description)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(250);

            RuleFor(e => e.Amount)
                .GreaterThan(0);

                RuleFor(e => e.CategoryId)
                    .GreaterThan(0)
                    // custom validation message for forms
                    // 0 means category is not choosen
                    .WithMessage(ValidatorOptions.Global.LanguageManager.GetString("NotEmptyValidator"));
        }
    }
}

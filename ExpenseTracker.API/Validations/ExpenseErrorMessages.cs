using ExpenseTracker.API.DTO;

namespace ExpenseTracker.API.Validations
{
    public static class ExpenseErrorMessages
    {
        public static ErrorMessage DescriptionCannotBeEmpty()
        {
            return new ErrorMessage("EXPENSE_DESCRIPTION_CANNOT_BE_EMPTY", "Description cannot be empty.");
        }

        public static ErrorMessage DescriptionTooLong(int maxCharactersLength, int currentCharactersLength)
        {
            return new ErrorMessage("EXPENSE_DESCRIPTION_TOO_LONG", $"The description is too long ({currentCharactersLength} characters). The maximum allowed length is {maxCharactersLength}.",
                new Dictionary<string, object>
                {
                    { "MaxCharactersLength", maxCharactersLength },
                    { "CurrentCharactersLength", currentCharactersLength }
                });
        }

        public static ErrorMessage AmountMustBeGreaterThanZero()
        {
            return new ErrorMessage("EXPENSE_AMOUNT_GREATER_THAN_ZERO", "Amount must be greater than zero.");
        }

        public static ErrorMessage NotFound(int id)
        {
            return new ErrorMessage("EXPENSE_NOT_FOUND", $"Expense with id '{id}' was not found",
                new Dictionary<string, object>
                {
                    { "Id", id }
                });
        }

        public static ErrorMessage AmountExceedsBudget(decimal amount, decimal budget, decimal totalExpenses)
        {
            return new ErrorMessage("EXPENSE_AMOUNT_EXCEEDS_BUDGET", $"Amount '{amount}' exceeds the budget '{budget}', total expensens '{totalExpenses}'",
                new Dictionary<string, object>
                {
                    { "Amount", amount },
                    { "Budget", budget },
                    { "TotalExpenses", totalExpenses }
                });
        }
    }
}

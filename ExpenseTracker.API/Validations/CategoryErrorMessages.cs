using ExpenseTracker.API.DTO;

namespace ExpenseTracker.API.Validations
{
    public static class CategoryErrorMessages
    {
        public static ErrorMessage LowerBudgetThanTotalExpenses(decimal budget, decimal totalExpenses)
        {
            return new ErrorMessage("CATEGORY_LOWER_BUDGET_THAN_TOTAL_EXPENSES", $"New budget '{budget}' is lower than the current total expenses '{totalExpenses}'. Reduce expenses first.",
                new Dictionary<string, object>
                {
                    { "Budget", budget },
                    { "TotalExpenses", totalExpenses }
                });
        }

        public static ErrorMessage CannotDeleteCategoryWithExpenses()
        {
            return new ErrorMessage("CATEGORY_CANNOT_DELETE_WITH_EXPENSES", "Cannot delete category assigned to expenses");
        }

        public static ErrorMessage NotFound(int id)
        {
            return new ErrorMessage("CATEGORY_NOT_FOUND", $"Category with id '{id}' not found",
                new Dictionary<string, object>
                {
                    { "Id", id }
                });
        }

        public static ErrorMessage NameTooLong(int maxCharactersLength, int currentCharactersLength)
        {
            return new ErrorMessage("CATEGORY_NAME_TOO_LONG", $"The category name is too long ({currentCharactersLength} characters). The maximum allowed length is {maxCharactersLength}.",
                new Dictionary<string, object>
                {
                    { "MaxCharactersLength", maxCharactersLength },
                    { "CurrentCharactersLength", currentCharactersLength }
                });
        }
    }
}

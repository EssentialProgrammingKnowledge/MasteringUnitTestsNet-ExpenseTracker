namespace ExpenseTracker.API.DTO
{
    public record ValidationResult(bool Success, ErrorMessage? ErrorMessage = null)
    {
        public static ValidationResult SuccessResult() => new(true);
        public static ValidationResult FailureResult(ErrorMessage errorMessage) => new(false, errorMessage);
    }
}

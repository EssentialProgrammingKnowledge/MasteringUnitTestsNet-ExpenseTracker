namespace ExpenseTracker.UI.Models
{
    public record Result(bool Valid, ErrorMessage? ErrorMessage = null)
    {
        public static Result Success()
        {
            return new Result(true);
        }

        public static Result Failed(ErrorMessage? errorMessage)
        {
            return new Result(false, errorMessage);
        }
    }

    public record Result<T>(bool Valid, ErrorMessage? ErrorMessage, T? Data)
    {
        public static Result<T> Success(T? data)
        {
            return new Result<T>(true, null, data);
        }

        public static Result<T> Failed(ErrorMessage? errorMessage)
        {
            return new Result<T>(false, errorMessage, default);
        }
    }
}

namespace ExpenseTracker.UI.Models
{
    public record ErrorMessage(string Code, string Message, Dictionary<string, object>? Parameters = null);
}

using ExpenseTracker.UI.Models;

namespace ExpenseTracker.UI.Languages
{
    public interface ITranslateService
    {
        string Translate(ErrorMessage errorMessage);
        string Translate(string translationKey, Dictionary<string, object>? parameters = null);
    }
}

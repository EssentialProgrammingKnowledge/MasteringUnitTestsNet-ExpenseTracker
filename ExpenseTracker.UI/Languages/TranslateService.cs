using ExpenseTracker.UI.Models;
using System.Collections.Frozen;
using System.Text;

namespace ExpenseTracker.UI.Languages
{
    public class TranslateService : ITranslateService
    {
        private readonly FrozenDictionary<string, string> _translations = new Dictionary<string, string>()
        {
            { "CATEGORY_LOWER_BUDGET_THAN_TOTAL_EXPENSES", "Nowy budżet '{Budget}' jest niższy niż obecne całkowite wydatki '{TotalExpenses}'. Najpierw zmniejsz wydatki." },
            { "CATEGORY_CANNOT_DELETE_WITH_EXPENSES", "Nie można usunąć kategorii przypisanej do wydatków." },
            { "CATEGORY_NOT_FOUND", "Nie znaleziono kategorii o identyfikatorze '{Id}'." },
            { "EXPENSE_DESCRIPTION_CANNOT_BE_EMPTY", "Opis nie może być pusty." },
            { "EXPENSE_DESCRIPTION_TOO_LONG", "Opis jest za długi ('{CurrentCharactersLength}'). Maksymalna długość to '{MaxCharactersLength}'." },
            { "EXPENSE_AMOUNT_GREATER_THAN_ZERO", "Kwota musi być większa niż zero." },
            { "EXPENSE_NOT_FOUND", "Nie znaleziono wydatku o identyfikatorze '{Id}'." },
            { "EXPENSE_AMOUNT_EXCEEDS_BUDGET", "Kwota '{Amount}' przekracza budżet '{Budget}', całkowite wydatki '{TotalExpenses}'." },
            { "GENERAL_ERROR", "Coś poszło nie tak, spróbuj ponownie później." }
        }.ToFrozenDictionary();

        public string Translate(ErrorMessage errorMessage)
        {
            if (errorMessage is null)
            {
                return string.Empty;
            }
            return Translate(errorMessage.Code, errorMessage.Parameters);
        }

        public string Translate(string translationKey, Dictionary<string, object>? parameters = null)
        {
            if (string.IsNullOrWhiteSpace(translationKey))
            {
                return string.Empty;
            }

            _translations.TryGetValue(translationKey, out var translatedText);
            if (translatedText is null)
            {
                return translationKey;
            }

            return ReplaceParameters(translatedText, parameters);
        }

        private string ReplaceParameters(string translatedText, Dictionary<string, object>? parameters = null)
        {
            if (parameters is null || parameters.Count == 0)
            {
                return translatedText;
            }

            var translatedTextWithReplaceParams = new StringBuilder(translatedText);
            foreach (var param in parameters)
            {
                if (param.Key is null || param.Value is null)
                {
                    continue;
                }

                translatedTextWithReplaceParams.Replace($"{{{param.Key}}}", param.Value?.ToString() ?? "null");
            }

            return translatedTextWithReplaceParams.ToString();
        }
    }
}

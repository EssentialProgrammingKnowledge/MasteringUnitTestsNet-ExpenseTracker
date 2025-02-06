using System.Globalization;

namespace ExpenseTracker.UI
{
    public static class Extensions
    {
        public static string ToCurrencyString(this decimal value)
        {
            return value.ToString("C", new CultureInfo("pl-PL"));
        }
    }
}

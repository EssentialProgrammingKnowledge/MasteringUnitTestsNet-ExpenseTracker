namespace ExpenseTracker.UI.Languages
{
    public static class Extensions
    {
        public static IServiceCollection AddTranslations(this IServiceCollection services)
        {
            return services.AddSingleton<ITranslateService, TranslateService>();
        }
    }
}

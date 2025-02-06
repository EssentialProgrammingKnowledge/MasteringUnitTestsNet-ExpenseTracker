namespace ExpenseTracker.UI.Services
{
    public static class Extensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            return services.AddScoped<ICategoryService, CategoryService>()
                           .AddScoped<IExpenseService, ExpenseService>();
        }
    }
}

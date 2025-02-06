namespace ExpenseTracker.API.Services
{
    public static class Extensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IExpenseService, ExpenseService>();
            return services;
        }
    }
}

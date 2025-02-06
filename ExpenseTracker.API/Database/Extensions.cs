using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.API.Database
{
    public static class Extensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services)
        {
            services.AddDbContext<ExpenseContext>(options =>
                options.UseInMemoryDatabase("ExpenseDb"));
            services.AddTransient<ISeedDataProvider, SeedDataProvider>();
            services.AddHostedService<DbInitializer>();
            return services;
        }
    }
}

namespace ExpenseTracker.API.Database
{
    public interface ISeedDataProvider
    {
        Task SeedData(CancellationToken cancellationToken = default);
    }
}

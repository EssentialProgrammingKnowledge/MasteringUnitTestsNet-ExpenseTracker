using Bunit;
using MudBlazor.Services;

namespace ExpenseTracker.UI.UnitTests.Common
{
    public class ConfiguredTestContext : TestContext
    {
        public ConfiguredTestContext()
        {
            Services.AddMudServices();
            JSInterop.Mode = JSRuntimeMode.Loose;
        }
    }
}

using Bunit;
using ExpenseTracker.UI.UnitTests.Common;
using MudBlazor;
using Shouldly;

namespace ExpenseTracker.UI.UnitTests.Components
{
    public class ExpensesComponentTests
    {
        private readonly TestContext _testContext;

        public ExpensesComponentTests()
        {
            _testContext = new ConfiguredTestContext();
            var popoverProvider = _testContext.RenderComponent<MudPopoverProvider>();
            popoverProvider.ShouldNotBeNull();
            popoverProvider.Markup.ShouldNotBeNull();
        }
    }
}

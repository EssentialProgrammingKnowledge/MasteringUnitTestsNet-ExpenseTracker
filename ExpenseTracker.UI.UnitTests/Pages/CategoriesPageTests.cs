using Bunit;
using ExpenseTracker.UI.Components;
using ExpenseTracker.UI.Pages;
using ExpenseTracker.UI.UnitTests.Common;
using MudBlazor.Services;
using Shouldly;

namespace ExpenseTracker.UI.UnitTests.Pages
{
    public class CategoriesPageTests
    {
        [Fact]
        public void ShouldRenderCategoriesPage()
        {
            // Arrange Act
            var component = _testContext.RenderComponent<Categories>();

            // Assert
            component.ShouldNotBeNull();
            component.Markup.ShouldNotBeNullOrWhiteSpace();
        }

        private readonly TestContext _testContext;

        public CategoriesPageTests()
        {
            _testContext = new ConfiguredTestContext();
            _testContext.Services.AddMudServices();
            _testContext.ComponentFactories.Add(type => type == typeof(CategoriesComponent),
                _ => new DummyComponent());
        }
    }
}

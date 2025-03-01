using Bunit;
using ExpenseTracker.UI.Components;
using ExpenseTracker.UI.Pages;
using ExpenseTracker.UI.UnitTests.Common;
using MudBlazor.Services;
using Shouldly;

namespace ExpenseTracker.UI.UnitTests.Pages
{
    public class HomePageTests
    {
        [Fact]
        public void ShouldRenderHomePage()
        {
            // Arrange Act
            var component = _testContext.RenderComponent<Home>();

            // Assert
            component.ShouldNotBeNull();
            component.Markup.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public void ShouldRenderPageWithInformation()
        {
            // Arrange Act
            var component = _testContext.RenderComponent<Home>();

            // Assert
            var mudPaper = component.Find("[data-name='home-page-info']");
            var text = component.Find("h3").TextContent;
            text.ShouldBe("Witaj w aplikacji zarządzającej wydatkami");
            mudPaper.ClassList.ShouldContain("pa-4");
            mudPaper.ClassList.ShouldContain("mb-2");
        }

        private readonly TestContext _testContext;

        public HomePageTests()
        {
            _testContext = new ConfiguredTestContext();
            _testContext.Services.AddMudServices();
            _testContext.ComponentFactories.Add(type => type == typeof(ExpensesComponent),
                _ => new DummyComponent());
        }
    }
}

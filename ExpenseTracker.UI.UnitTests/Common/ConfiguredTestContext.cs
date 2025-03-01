using Bunit;
using MudBlazor.Services;

namespace ExpenseTracker.UI.UnitTests.Common
{
    public class ConfiguredTestContext : TestContext
    {
        public ConfiguredTestContext()
        {
            Services.AddMudServices();
            JSInterop.SetupVoid("mudKeyInterceptor.connect", _ => true);
            JSInterop.SetupVoid("mudKeyInterceptor.disconnect", _ => true);
            JSInterop.SetupVoid("mudScrollManager.unlockScroll", _ => true);
            JSInterop.SetupVoid("mudPopover.initialize", _ => true);
            JSInterop.SetupVoid("mudElementRef.saveFocus", _ => true);
            JSInterop.SetupVoid("mudScrollManager.lockScroll", _ => true);
            JSInterop.SetupVoid("mudPopover.connect", _ => true);
            JSInterop.Setup<int>("mudpopoverHelper.countProviders").SetResult(1);
        }
    }
}

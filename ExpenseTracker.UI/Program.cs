using ExpenseTracker.UI;
using ExpenseTracker.UI.Languages;
using ExpenseTracker.UI.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration.GetValue<string>("Backend") ?? throw new InvalidOperationException("Backend url was not provided")) });
builder.Services.AddMudServices();
builder.Services.AddServices();
builder.Services.AddTranslations();

await builder.Build().RunAsync();

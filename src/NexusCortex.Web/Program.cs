using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NexusCortex.Web;
using NexusCortex.Web.Services;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Assuming API runs on port 5123 based on previous run commands
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5123/") });
builder.Services.AddScoped<IDashboardApiClient, DashboardApiClient>();
builder.Services.AddScoped<INodeApiClient, NodeApiClient>();

builder.Services.AddMudServices();

await builder.Build().RunAsync();

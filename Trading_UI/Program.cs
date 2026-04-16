using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Trading_UI.Components;
using Trading_UI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<IFyersDataService, FyersDataService>();
builder.Services.AddScoped<IHistoricalDataService, HistoricalDataService>();
builder.Services.AddScoped<IOptionChainService, OptionChainService>();

var apiBaseAddress = builder.Configuration["ApiSettings:BaseAddress"] ?? "https://localhost:7203";

builder.Services.AddHttpClient<IFyersDataService, FyersDataService>(client =>
{
    client.BaseAddress = new Uri(apiBaseAddress);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient<IHistoricalDataService, HistoricalDataService>(client =>
{
    client.BaseAddress = new Uri(apiBaseAddress);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient<IOptionChainService, OptionChainService>(client =>
{
    client.BaseAddress = new Uri(apiBaseAddress);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

using Trading.Domain.Services;
using Trading.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddControllers();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Register Trading Services
builder.Services.AddScoped<IMarketDataService, MockMarketDataService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ITradeService, TradeService>();
builder.Services.AddScoped<ITradeLogService, TradeLogService>();
builder.Services.AddScoped<IAutomatedTradingService, AutomatedTradingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.MapControllers();

app.Run();


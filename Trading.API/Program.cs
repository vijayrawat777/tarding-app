using Microsoft.OpenApi.Models;
using Trading.Infrastructure.Configuration;
using Trading.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddLogging();
builder.Services.AddHttpClient();

// Add Swagger/OpenAPI
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Trading API",
        Version = "v1",
        Description = "Algo Trading Web Application API - Fyers Integration",
        Contact = new OpenApiContact
        {
            Name = "Trading Team",
            Email = "support@tradingapp.com"
        }
    });

    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Fyers Access Token - obtained from /api/auth/get-access-token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });

    // Add XML comments for API docs
    var xmlFile = "Trading.API.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var fyersConfig = builder.Configuration.GetSection("FyersConfig").Get<FyersConfig>();
if (fyersConfig != null)
{
    builder.Services.AddSingleton(fyersConfig);
    builder.Services.AddScoped<IFyersAuthenticationService, FyersAuthenticationService>();
    builder.Services.AddScoped<IFyersOptionChainService, FyersOptionChainService>();
}
builder.Services.AddScoped<IOptionStrategyEngine, OptionStrategyEngine>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.SerializeAsV2 = false;
    });

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Trading API V1");
        options.RoutePrefix = "swagger";
    });

    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.MapControllers();

app.Run();

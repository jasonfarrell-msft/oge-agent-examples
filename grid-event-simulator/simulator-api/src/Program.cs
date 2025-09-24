using System.Text.Json;
using GridSimulator.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddTransient<IRunSimulationService, DefaultRunSimulationService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Configure JSON options for proper binding
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.AllowTrailingCommas = true;
    });

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseRouting();
app.MapControllers();

app.Run();

using System.Text.Json;
using GridSimulator.Api;
using GridSimulator.Api.Clients;
using GridSimulator.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddSingleton<IAgentFactory, AgentFactory>();
builder.Services.AddSingleton<IKernelFactory, KernelFactory>();
builder.Services.AddTransient<IRunSimulationService, DefaultRunSimulationService>();

builder.Services.AddHttpClient<IRatesApiHttpClient, RatesApiHttpClient>(options =>
{
    options.BaseAddress = new Uri("https://func-rate-api-eus2-mx01.azurewebsites.net");
    options.Timeout = TimeSpan.FromSeconds(120);
});

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

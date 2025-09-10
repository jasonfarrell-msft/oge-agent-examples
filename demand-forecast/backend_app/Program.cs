using Farrellsoft.Examples.Agents.MultiAgent.Services;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();
builder.Services.AddTransient<ICacheService, RedisCacheService>();
builder.Services.AddTransient<IAgentService, DefaultAgentService>();

builder.Build().Run();

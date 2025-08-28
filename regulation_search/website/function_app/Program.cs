using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Azure.AI.Projects;
using Azure.Identity;
using Farrellsoft.Examples.Agents.SingleAgent.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        // Register logging
        services.AddLogging();

        services.AddScoped<IProcessQueryService, ProcessQueryService>();
        services.AddScoped<IAgentFileService, AgentFileService>();
    })
    .Build();

host.Run();

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Azure.AI.Projects;
using Azure.Identity;
using Farrellsoft.Examples.Agents.SingleAgent.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        // Register configuration
        var configuration = context.Configuration;
        
        // Register Azure AI Projects client with DefaultAzureCredential
        services.AddSingleton<AIProjectClient>(serviceProvider =>
        {
            var endpoint = configuration["FoundryProjectEndpoint"] 
                ?? throw new InvalidOperationException("FoundryProjectEndpoint configuration is required");
            
            var credential = new DefaultAzureCredential();
            return new AIProjectClient(endpoint, credential);
        });
        
        // Register logging
        services.AddLogging();
        
        services.AddScoped<IProcessQueryService, ProcessQueryService>();
    })
    .Build();

host.Run();

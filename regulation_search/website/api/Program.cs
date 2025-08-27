using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Farrellsoft.Examples.Agents.SingleAgent.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddScoped<IProcessQueryService, ProcessQueryService>();
    })
    .Build();

host.Run();

using GridSimulator.Api.Clients;
using GridSimulator.Api.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

namespace GridSimulator.Api;

public class KernelFactory(IConfiguration configuration, IServiceProvider serviceProvider) : IKernelFactory
{
    public Kernel GetKernel()
    {
        var apiKey = configuration["API_KEY"] ?? throw new InvalidOperationException("API_KEY configuration is missing");
        var kernel = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(
                deploymentName: "gpt-5-mini-deployment",
                endpoint: "https://orch-multi-agent-resource.cognitiveservices.azure.com",
                apiKey: apiKey,
                serviceId: "generative-service")
            .AddAzureOpenAIChatCompletion(
                deploymentName: "o4-mini-deployment",
                endpoint: "https://orch-multi-agent-resource.cognitiveservices.azure.com",
                apiKey: apiKey,
                serviceId: "reasoning-service")
            .Build();
        
        // add plugins using dependency injection
        var ratesHttpClient = serviceProvider.GetRequiredService<IRatesApiHttpClient>();
        var ratesPlugin = new RatesPlugin(ratesHttpClient);
        kernel.Plugins.AddFromObject(ratesPlugin);
        
        return kernel;
    }
}

public interface IKernelFactory
{
    Kernel GetKernel();
}
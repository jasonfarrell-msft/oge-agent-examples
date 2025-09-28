using GridSimulator.Api.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace GridSimulator.Api.Services;

public class DefaultRunSimulationService(IConfiguration configuration, IAgentFactory agentFactory,
    ILogger<DefaultRunSimulationService> logger) : IRunSimulationService
{    
    public async Task<string> RunSimulationAsync(RunSimulationRequestModel request)
    {
        try
        {
            logger.LogInformation("🚀 Starting grid simulation with sequential orchestration");
            Console.WriteLine("🚀 GRID SIMULATION STARTED - SEQUENTIAL ORCHESTRATION");
            
            var input = request.SimulationParameters.DemandIncreaseParameters is not null
                ? Prompts.GetDemandIncreaseSimulationInput()
                : Prompts.GetOutputReductionSimulationInput(request);

            if (string.IsNullOrEmpty(input))
            {
                throw new InvalidOperationException("Failed to generate simulation input - received null or empty input");
            }

            logger.LogInformation("📝 Input: {Input}", input);
            Console.WriteLine($"📝 INPUT: {input}");

            // Step 1: Calculate Demand
            logger.LogInformation("📊 Step 1: Calculating Demand");
            Console.WriteLine("📊 STEP 1: CALCULATING DEMAND");
            
            var demandResult = await InvokeAgentAsync(agentFactory.DemandCalculationAgent, input);
            
            if (string.IsNullOrEmpty(demandResult))
            {
                throw new InvalidOperationException("Demand calculation failed - no response received");
            }

            logger.LogInformation("✅ Step 1 Complete: {Preview}", 
                demandResult.Substring(0, Math.Min(150, demandResult.Length)) + "...");
            Console.WriteLine($"✅ STEP 1 COMPLETE");
            Console.WriteLine(new string('-', 80));

            // Step 2: Analyze deficit and determine strategies
            logger.LogInformation("🔍 Step 2: Analyzing Grid and Determining Strategies");
            Console.WriteLine("🔍 STEP 2: ANALYZING GRID AND DETERMINING STRATEGIES");
            
            var analysisPrompt = $@"Based on the demand calculation below, analyze the deficit and available strategies:

{demandResult}

{input}

Please analyze the demand calculation above and determine what actions are needed to cover any deficit.";

            var analysisResult = await InvokeAgentAsync(agentFactory.GridAnalysisAgent, analysisPrompt);
            
            if (string.IsNullOrEmpty(analysisResult))
            {
                throw new InvalidOperationException("Grid analysis failed - no response received");
            }

            logger.LogInformation("✅ Step 2 Complete: {Preview}", 
                analysisResult.Substring(0, Math.Min(150, analysisResult.Length)) + "...");
            Console.WriteLine($"✅ STEP 2 COMPLETE");
            Console.WriteLine(new string('-', 80));

            // Step 3: Create action plan
            logger.LogInformation("📋 Step 3: Creating Action Plan");
            Console.WriteLine("📋 STEP 3: CREATING ACTION PLAN");
            
            var actionPlanPrompt = $@"Based on the demand calculation and analysis below, create a comprehensive action plan:

DEMAND CALCULATION:
{demandResult}

GRID ANALYSIS:
{analysisResult}

Please create the comprehensive action plan based on the analysis above for use by a human operator.";

            var actionPlanResult = await InvokeAgentAsync(agentFactory.ActionPlanAgent, actionPlanPrompt);
            
            if (string.IsNullOrEmpty(actionPlanResult))
            {
                throw new InvalidOperationException("Action plan creation failed - no response received");
            }

            logger.LogInformation("✅ Step 3 Complete: {Preview}", 
                actionPlanResult.Substring(0, Math.Min(150, actionPlanResult.Length)) + "...");
            Console.WriteLine($"✅ STEP 3 COMPLETE");
            Console.WriteLine(new string('-', 80));

            // Compile final result
            var finalResult = $@"# Grid Simulation Results

## Demand Calculation
{demandResult}

## Grid Analysis
{analysisResult}

## Action Plan
{actionPlanResult}";

            logger.LogInformation("🏁 Sequential simulation completed successfully");
            Console.WriteLine("🏁 SEQUENTIAL SIMULATION COMPLETED SUCCESSFULLY");
            
            return finalResult;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Error occurred during simulation");
            Console.WriteLine($"❌ ERROR: {ex.Message}");
            return $"Error: {ex.Message}";
        }
    }

    private async Task<string> InvokeAgentAsync(ChatCompletionAgent agent, string input)
    {
        try
        {
#pragma warning disable SKEXP0110
            var chat = new AgentGroupChat(agent);
            chat.AddChatMessage(new ChatMessageContent(AuthorRole.User, input));

            ChatMessageContent? lastResponse = null;
            await foreach (var response in chat.InvokeAsync())
            {
                lastResponse = response;
                logger.LogInformation("💬 Agent {Agent} responded: {Preview}", 
                    response.AuthorName ?? "Unknown",
                    response.Content?.Substring(0, Math.Min(150, response.Content?.Length ?? 0)) + "...");
                
                Console.WriteLine($"💬 AGENT RESPONSE [{response.AuthorName ?? "Unknown"}]:");
                Console.WriteLine($"   {response.Content}");
                break; // Only get the first response for sequential processing
            }

            return lastResponse?.Content ?? string.Empty;
#pragma warning restore SKEXP0110
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Error invoking agent {AgentName}", agent.Name);
            throw;
        }
    }
}

public interface IRunSimulationService
{
    Task<string> RunSimulationAsync(RunSimulationRequestModel request);
}
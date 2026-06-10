using Clinic.Application.DTOs;
using Clinic.Application.Interfaces;
using Clinic.Domain;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using static Clinic.Domain.Enums.ConversationEnums;

namespace Clinic.Application.Services
{
    public class ChatService(
        IConversationRepository conversationRepository,
        ILLMProvider llmProvider,
        ConversationToolRegistry toolRegistry,
        ILogger<ChatService> logger
        ) : IChatService
    {
        //It should be configurable:
        private const int CONFIG_MAX_TOOL_LOOP_ITERATIONS = 10;

        private const string CONFIG_WELCOME_MESSAGE = @"Hello!
            Welcome to CLINIC_AI.
            I'm the clinic's virtual assistant and can help you with appointment scheduling, rescheduling, cancellations, clinic information, and general questions.
            How may I assist you today?
            ";
        private const string CONFIG_SYSTEM_PROMPT = @"You are the official virtual assistant for CLINIC_AI.

            Your role is to assist patients with appointment scheduling, rescheduling, cancellations, clinic information, and general inquiries.

            Guidelines:

            * Always communicate in a professional, friendly, and concise manner.
            * Respond in the same language used by the patient whenever possible.
            * Ask only for the information necessary to complete the requested task.
            * Prefer asking one question at a time when collecting patient information.
            * Use the conversation history to avoid repeating questions.

            Restrictions:

            * Do not provide medical diagnoses.
            * Do not prescribe medications or treatments.
            * Do not invent information about doctors, schedules, services, pricing, or insurance coverage.
            * If information is unavailable, explain that a member of the clinic staff will assist further.            
            * If the patient describes a medical emergency, advise them to contact emergency services or seek immediate medical attention.

            Workflow:

            1. Identify the patient's request.
            2. Gather the required information.
            3. Confirm important details when necessary.
            4. Complete or register the request.
            5. Clearly explain the next step.

            Tone:

            * Professional
            * Helpful
            * Empathetic
            * Efficient
            ";

        public async Task<ChatMessageResponseDto> HandleMessageAsync(ChatMessageDto message, CancellationToken ct = default)
        {
            var conversation = message.ConversationId.HasValue
                ? await conversationRepository.GetByIdAsync(message.ConversationId.Value, ct)
                : null;

            if (conversation is null)
            {
                conversation = Conversation.Create(
                    message.ContactName, message.ContactPhone);

                conversation = await conversationRepository.AddAsync(conversation, ct);

                var welcomeMsg = ConversationMessage.Create(
                    conversation.Id, MessageRole.Assistant, CONFIG_WELCOME_MESSAGE);
                conversation.AddMessage(welcomeMsg);
            }

            var userMsg = ConversationMessage.Create(
                conversation.Id, MessageRole.User, message.Content);
            conversation.AddMessage(userMsg);

            var context = new ConversationContextDto(
               conversation.Id,
               conversation.ContactName,
               conversation.ContactPhone);

            var llmMessage = await RunToolLoopAsync(conversation, message.Content, context, ct);

            var assistantMsg = ConversationMessage.Create(
                conversation.Id, MessageRole.Assistant, llmMessage);
            conversation.AddMessage(assistantMsg);

            await conversationRepository.UpdateAsync(conversation, ct);

            return new ChatMessageResponseDto(conversation.Id, llmMessage);
        }



        internal async Task<string> RunToolLoopAsync(
            Conversation conversation,
            string userMessage,
            ConversationContextDto context,
            CancellationToken ct)
        {
            var tools = toolRegistry.GetAllTools();
            var toolDefinitions = tools.Select(t =>
                new LlmToolDefinition(t.Name, t.Description, t.ParametersJsonSchema)).ToList();

            var request = new LlmChatRequest
            {
                SystemPrompt = CONFIG_SYSTEM_PROMPT,
                History = conversation.Messages,
                UserMessage = userMessage,
                Tools = toolDefinitions
            };

            for (var i = 0; i < CONFIG_MAX_TOOL_LOOP_ITERATIONS; i++)
            {
                var response = await llmProvider.ChatAsync(request, ct);

                if (response.StopReason != LlmStopReason.ToolUse || response.ToolCalls.Count == 0)
                {
                    return response.TextContent ?? string.Empty;
                }

                var toolResults = new List<LlmToolResult>();
                foreach (var toolCall in response.ToolCalls)
                {
                    var tool = toolRegistry.GetTool(toolCall.Name);
                    if (tool is null)
                    {
                        logger.LogWarning("LLM requested unknown tool: {ToolName}", toolCall.Name);
                        toolResults.Add(new LlmToolResult(
                            toolCall.Id, toolCall.Name,
                            JsonSerializer.Serialize(new { error = $"Unknown tool: {toolCall.Name}" }),
                            IsError: true));
                        continue;
                    }

                    try
                    {
                        var result = await tool.ExecuteAsync(toolCall.ArgumentsJson, context, ct);
                        toolResults.Add(new LlmToolResult(toolCall.Id, toolCall.Name, result));
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Tool {ToolName} execution failed", toolCall.Name);
                        toolResults.Add(new LlmToolResult(
                            toolCall.Id, toolCall.Name,
                            JsonSerializer.Serialize(new { error = ex.Message }),
                            IsError: true));
                    }
                }

                request = new LlmChatRequest
                {
                    SystemPrompt = CONFIG_SYSTEM_PROMPT,
                    History = conversation.Messages,
                    UserMessage = userMessage,
                    Tools = toolDefinitions,
                    PreviousAssistantToolCalls = response.ToolCalls,
                    ToolResults = toolResults
                };
            }

            logger.LogWarning("Tool loop reached max iterations ({Max})", CONFIG_MAX_TOOL_LOOP_ITERATIONS);
            return "[Max tool iterations reached] Could not complete the request.";
        }
    }
}

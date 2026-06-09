using Clinic.Application.DTOs;
using Clinic.Application.Interfaces;
using Clinic.Domain;
using static Clinic.Domain.Enums.ConversationEnums;

namespace Clinic.Application.Services
{
    public class ChatService(
        IConversationRepository conversationRepository,
        ILLMProvider llmProvider
        ) : IChatService
    {
        //It should be configurable:
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

            var llmMessage = await CallChatLlm(message.Content, conversation);

            var assistantMsg = ConversationMessage.Create(
                conversation.Id, MessageRole.Assistant, llmMessage);
            conversation.AddMessage(assistantMsg);

            await conversationRepository.UpdateAsync(conversation, ct);

            return new ChatMessageResponseDto(conversation.Id, llmMessage);
        }

        private async Task<string> CallChatLlm(string userMessage, Conversation conversation)
        {
            var request = new LlmChatRequest
            {
                SystemPrompt = CONFIG_SYSTEM_PROMPT,
                History = conversation.Messages,
                UserMessage = userMessage
            };

            var response = await llmProvider.ChatAsync(request);
            return response?.TextContent ?? "";
        }
    }
}

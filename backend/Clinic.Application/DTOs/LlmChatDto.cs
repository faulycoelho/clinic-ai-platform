using Clinic.Domain;

namespace Clinic.Application.DTOs
{
    public sealed class LlmChatRequest
    {
        public required string SystemPrompt { get; init; }
        public required IReadOnlyList<ConversationMessage> History { get; init; }
        public required string UserMessage { get; init; }
    }

    public sealed class LlmResponse
    {
        public required string? TextContent { get; init; }
    }
}

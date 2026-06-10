using Clinic.Domain;

namespace Clinic.Application.DTOs
{
    public sealed class LlmChatRequest
    {
        public required string SystemPrompt { get; init; }
        public required IReadOnlyList<ConversationMessage> History { get; init; }
        public required string UserMessage { get; init; }
        public required IReadOnlyList<LlmToolDefinition> Tools { get; init; }
        public IReadOnlyList<LlmToolCall>? PreviousAssistantToolCalls { get; init; }
        public IReadOnlyList<LlmToolResult>? ToolResults { get; init; }
    }

    public sealed class LlmResponse
    {
        public required string? TextContent { get; init; }
        public required IReadOnlyList<LlmToolCall> ToolCalls { get; init; }
        public bool IsToolUse => ToolCalls.Count > 0;
        public required LlmStopReason StopReason { get; init; }
    }

    public sealed record LlmToolCall(string Id, string Name, string ArgumentsJson, string? RawPartJson = null);
    public sealed record LlmToolResult(
    string ToolCallId,
    string ToolName,
    string ContentJson,
    bool IsError = false);

    public sealed record LlmToolDefinition(
        string Name,
        string Description,
        string ParametersJsonSchema);

    public enum LlmStopReason
    {
        EndTurn,
        ToolUse,
        MaxTokens,
        Error
    }
}

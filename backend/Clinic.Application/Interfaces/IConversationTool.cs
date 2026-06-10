using Clinic.Application.DTOs;

namespace Clinic.Application.Interfaces
{
    public interface IConversationTool
    {
        string Name { get; }
        string Description { get; }
        string ParametersJsonSchema { get; }
        Task<string> ExecuteAsync(string argumentsJson, ConversationContextDto? context = null, CancellationToken ct = default);
    }
}

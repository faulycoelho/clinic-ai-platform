using Clinic.Application.DTOs;

namespace Clinic.Application.Interfaces
{
    public interface ILLMProvider
    {
        Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default);
        Task<LlmResponse> ChatAsync(LlmChatRequest request, CancellationToken ct = default);
    }
}

namespace Clinic.Application.Interfaces
{
    public interface ILLMProvider
    {
        Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default);
    }
}

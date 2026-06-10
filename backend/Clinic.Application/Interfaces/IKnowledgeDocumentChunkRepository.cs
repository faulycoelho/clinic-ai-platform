using Clinic.Domain;

namespace Clinic.Application.Interfaces
{
    public interface IKnowledgeDocumentChunkRepository
    {
        Task<KnowledgeDocumentChunk> AddAsync(KnowledgeDocumentChunk doc, CancellationToken ct = default);
        Task<KnowledgeDocumentChunk[]> AddRangeAsync(KnowledgeDocumentChunk[] docChunk, CancellationToken ct = default);
        Task<List<string>> SearchByVectorAsync(float[] queryEmbedding, int topK = 3);
    }
}

using Clinic.Domain;

namespace Clinic.Application.Interfaces
{
    public interface IKnowledgeDocumentChunkRepository
    {
        Task<KnowledgeDocumentChunk> AddAsync(KnowledgeDocumentChunk doc, CancellationToken ct = default);
        Task<IEnumerable<KnowledgeDocumentChunk>> SearchByVectorAsync(float[] queryEmbedding, int topK = 3);
    }
}

using Clinic.Application.Interfaces;
using Clinic.Domain;
using Clinic.Infrastructure.Data;

namespace Clinic.Infrastructure.Repositories
{
    public class KnowledgeDocumentChunkRepository(AppDbContext db) : IKnowledgeDocumentChunkRepository
    {
        public async Task<KnowledgeDocumentChunk> AddAsync(KnowledgeDocumentChunk docChunk, CancellationToken ct = default)
        {
            db.KnowledgeDocumentChunks.Add(docChunk);
            await db.SaveChangesAsync(ct);
            return docChunk;
        }

        public async Task<KnowledgeDocumentChunk[]> AddRangeAsync(KnowledgeDocumentChunk[] docChunk, CancellationToken ct = default)
        {
            db.KnowledgeDocumentChunks.AddRange(docChunk);
            await db.SaveChangesAsync(ct);
            return docChunk;
        }
    }
}

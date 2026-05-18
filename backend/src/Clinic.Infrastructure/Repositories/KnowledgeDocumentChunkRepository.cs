using Clinic.Application.Interfaces;
using Clinic.Domain;
using Clinic.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


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

        public async Task<IEnumerable<KnowledgeDocumentChunk>> SearchByVectorAsync(float[] queryEmbedding, int topK = 3)
        {
            var queryVector = new Pgvector.Vector(queryEmbedding);

            return await db.KnowledgeDocumentChunks
                .FromSqlRaw(
                    """
                SELECT * FROM documents
                WHERE embedding IS NOT NULL
                ORDER BY embedding <=> {0}::vector
                LIMIT {1}
                """,
                    queryVector, topK)
                .ToListAsync();
        }
    }
}

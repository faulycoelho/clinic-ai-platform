using Clinic.Application.Interfaces;
using Clinic.Domain;
using Clinic.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace Clinic.Infrastructure.Repositories
{
    public class KnowledgeDocumentChunkRepository(AppDbContext db, IConfiguration config) : IKnowledgeDocumentChunkRepository
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
        public async Task<List<string>> SearchByVectorAsync(float[] queryEmbedding, int topK = 3)
        {
            return await db.Database
            .SqlQuery<string>(
                $"""
                SELECT "Content"
                FROM "KnowledgeDocumentChunks"
                WHERE "Embedding" IS NOT NULL
                ORDER BY "Embedding" <=> {queryEmbedding}::vector
                LIMIT {topK}
                """)
             .ToListAsync();
        }
    }
}

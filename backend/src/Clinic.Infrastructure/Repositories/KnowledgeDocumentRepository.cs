using Clinic.Application.DTOs;
using Clinic.Application.Interfaces;
using Clinic.Domain;
using Clinic.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Infrastructure.Repositories
{

    public class KnowledgeDocumentRepository(AppDbContext db) : IKnowledgeDocumentRepository
    {
        public async Task<KnowledgeDocument?> GetByIdAsync(int id, CancellationToken ct = default)
            => await db.KnowledgeDocuments.FindAsync([id], ct);

        public async Task<IReadOnlyList<KnowledgeDocument>> GetAllActiveAsync(CancellationToken ct = default)
            => await db.KnowledgeDocuments
                .Where(d => d.IsActive)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync(ct);

        public async Task<IReadOnlyList<KnowledgeDocument>> SearchAsync(string query, CancellationToken ct = default)
        {
            var pattern = $"%{query}%";
            return await db.KnowledgeDocuments
                .Where(d => d.IsActive &&
                    (EF.Functions.ILike(d.Title, pattern) ||
                     (d.Description != null && EF.Functions.ILike(d.Description, pattern))))
                .OrderByDescending(d => d.CreatedAt)
                .Take(10)
                .ToListAsync(ct);
        }

        public async Task<KnowledgeDocument> AddAsync(KnowledgeDocument doc, CancellationToken ct = default)
        {
            db.KnowledgeDocuments.Add(doc);
            await db.SaveChangesAsync(ct);
            return doc;
        }

        public async Task<KnowledgeDocument> UpdateAsync(KnowledgeDocument doc, CancellationToken ct = default)
        {
            db.KnowledgeDocuments.Update(doc);
            await db.SaveChangesAsync(ct);
            return doc;
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var doc = await db.KnowledgeDocuments.FindAsync([id], ct);
            if (doc is not null)
            {
                db.KnowledgeDocuments.Remove(doc);
                await db.SaveChangesAsync(ct);
            }
        }

        public async Task<bool> HasAnyAsync(CancellationToken ct = default)
            => await db.KnowledgeDocuments.AnyAsync(ct);

        public async Task<List<DocumentDto>> SearchByVectorAsync(float[] queryEmbedding, int topK = 3)
        {
            return await db.Database
             .SqlQuery<DocumentDto>(
                 $"""
                SELECT
                    kd."Id" as "DocumentId",
                    kd."Title" as "DocumentTitle",
                    kdc."Content"
                FROM "KnowledgeDocuments" kd
                INNER JOIN "KnowledgeDocumentChunks" kdc ON kdc."KnowledgeDocumentId" = kd."Id"
                WHERE kd."IsActive" = TRUE
                  AND kd."ProcessingStatus" = {(int)Domain.Enums.DocumentProcessingStatus.Completed}
                  AND kdc."Embedding" IS NOT NULL
                ORDER BY kdc."Embedding" <=> {queryEmbedding}::vector
                LIMIT {topK}
                """)
             .ToListAsync();
        }
    }
}
using Clinic.Domain;

namespace Clinic.Application.Interfaces
{
    public interface IKnowledgeDocumentRepository
    {
        Task<KnowledgeDocument?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<KnowledgeDocument>> GetAllActiveAsync(CancellationToken ct = default);
        Task<IReadOnlyList<KnowledgeDocument>> SearchAsync(string query, CancellationToken ct = default);
        Task<KnowledgeDocument> AddAsync(KnowledgeDocument doc, CancellationToken ct = default);
        Task<KnowledgeDocument> UpdateAsync(KnowledgeDocument doc, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}

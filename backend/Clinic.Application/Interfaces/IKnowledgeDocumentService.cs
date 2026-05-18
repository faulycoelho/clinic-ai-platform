using Clinic.Application.DTOs;

namespace Clinic.Application.Interfaces
{
    public interface IKnowledgeDocumentService
    {
        Task<IReadOnlyList<KnowledgeDocumentDto>> GetAllAsync(CancellationToken ct = default);
        Task<KnowledgeDocumentDto> GetByIdAsync(int id, CancellationToken ct = default);
        Task<KnowledgeDocumentDto> UploadAsync(
            string title, string? description, string fileName,
            string contentType, Stream fileStream, long fileSize,
            CancellationToken ct = default);
        Task<KnowledgeDocumentDto> UpdateAsync(int id, UpdateKnowledgeDocumentDto dto, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}

namespace Clinic.Application.DTOs
{
    public record KnowledgeDocumentDto(
        int Id,
        string Title,
        string? Description,
        string FileName,
        string ContentType,
        long FileSizeBytes,
        bool IsActive,
        DateTime CreatedAt,
        DateTime? UpdatedAt);
     
    public record UpdateKnowledgeDocumentDto(
        string Title,
        string? Description,
        bool IsActive);
}

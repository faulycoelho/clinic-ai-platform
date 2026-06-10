namespace Clinic.Application.DTOs
{
    public sealed record DocumentDto(
        int DocumentId,
        string DocumentTitle,
        string Content);
}

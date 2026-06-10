namespace Clinic.Application.DTOs
{
    public record ConversationContextDto(
        int ConversationId,
        string? ContactName,
        string? ContactPhone
    );
}

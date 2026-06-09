using static Clinic.Domain.Enums.ConversationEnums;

namespace Clinic.Application.DTOs
{
    public record ConversationDto(
        int Id,
        string ContactName,
        string ContactPhone, 
        ConversationStatus Status,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        DateTime? ClosedAt);

    public record CreateConversationDto(
        string ContactName,
        string ContactPhone);

    public record ConversationDetailDto(
        int Id,
        string? ContactName,
        string? ContactPhone,
        ConversationStatus Status,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        DateTime? ClosedAt,
        IReadOnlyList<ConversationMessageDto> Messages);

    public record ConversationMessageDto(
        int Id,
        int ConversationId,
        MessageRole Role,
        string Content,
        DateTime CreatedAt);

    public record SendMessageDto(
        string Content);
}

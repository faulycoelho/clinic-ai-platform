namespace Clinic.Application.DTOs
{
    public sealed record ChatMessageDto(
        int? ConversationId,
        string ContactName,
        string ContactPhone,
        string Content);

    public sealed record ChatMessageResponseDto(
    int ConversationId,
    string Content);
}

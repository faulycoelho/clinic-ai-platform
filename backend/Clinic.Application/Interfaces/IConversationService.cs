using Clinic.Application.DTOs;

namespace Clinic.Application.Interfaces
{
    public interface IConversationService
    {
        Task<IReadOnlyList<ConversationDto>> GetAllAsync(CancellationToken ct = default);
        Task<ConversationDetailDto> GetByIdAsync(int id, CancellationToken ct = default);
        Task<ConversationDto> CreateAsync(CreateConversationDto dto, CancellationToken ct = default);
        Task<ConversationDetailDto> SendMessageAsync(int conversationId, SendMessageDto dto, CancellationToken ct = default);
        Task<ConversationDto> CloseAsync(int id, CancellationToken ct = default);
    }
}

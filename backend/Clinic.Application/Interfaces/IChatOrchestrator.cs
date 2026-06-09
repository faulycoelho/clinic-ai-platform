using Clinic.Application.DTOs;

namespace Clinic.Application.Interfaces
{
    public interface IChatService
    {
        Task<ChatMessageResponseDto> HandleMessageAsync(ChatMessageDto message, CancellationToken ct = default);
    }
}

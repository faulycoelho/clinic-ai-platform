using Clinic.Domain;

namespace Clinic.Application.Interfaces
{
    public interface IConversationRepository
    {
        Task<Conversation> AddAsync(Conversation conversation, CancellationToken ct = default);
        Task<Conversation> UpdateAsync(Conversation conversation, CancellationToken ct = default);
        Task<Conversation?> GetByIdWithMessagesAsync(int id, CancellationToken ct = default);
        Task<Conversation?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<Conversation>> GetAllAsync(CancellationToken ct = default);
    }
}

using Clinic.Application.DTOs;
using Clinic.Application.Interfaces;
using Clinic.Domain;

namespace Clinic.Application.Services
{
    public class ConversationService(IConversationRepository repo) : IConversationService
    {
        public async Task<IReadOnlyList<ConversationDto>> GetAllAsync(CancellationToken ct = default)
        {
            var conversations = await repo.GetAllAsync(ct);
            return conversations.Select(ToDto).ToList();
        }

        public async Task<ConversationDetailDto> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var conversation = await repo.GetByIdWithMessagesAsync(id, ct)
                ?? throw new KeyNotFoundException($"Conversation {id} not found.");

            return ToDetailDto(conversation);
        }

        public async Task<ConversationDto> CreateAsync(CreateConversationDto dto, CancellationToken ct = default)
        {
            var conversation = Conversation.Create(dto.ContactName, dto.ContactPhone);
            var saved = await repo.AddAsync(conversation, ct);
            return ToDto(saved);
        }

        public async Task<ConversationDetailDto> SendMessageAsync(
            int conversationId, SendMessageDto dto, CancellationToken ct = default)
        {
            var conversation = await repo.GetByIdWithMessagesAsync(conversationId, ct)
                ?? throw new KeyNotFoundException($"Conversation {conversationId} not found.");

            var message = ConversationMessage.Create(
                conversation.Id,  Domain.Enums.ConversationEnums.MessageRole.User, dto.Content);
            conversation.AddMessage(message);

            var saved = await repo.UpdateAsync(conversation, ct);
            return ToDetailDto(saved);
        }

        public async Task<ConversationDto> CloseAsync(int id, CancellationToken ct = default)
        {
            var conversation = await repo.GetByIdAsync(id, ct)
                ?? throw new KeyNotFoundException($"Conversation {id} not found.");

            conversation.Close();
            var saved = await repo.UpdateAsync(conversation, ct);
            return ToDto(saved);
        }

        private static ConversationDto ToDto(Conversation c) => new(
            c.Id, c.ContactName, c.ContactPhone,
            c.Status, c.CreatedAt, c.UpdatedAt, c.ClosedAt);

        private static ConversationDetailDto ToDetailDto(Conversation c) => new(
            c.Id, c.ContactName, c.ContactPhone,
            c.Status,
            c.CreatedAt, c.UpdatedAt, c.ClosedAt,
            c.Messages.Select(ToMessageDto).ToList());

        private static ConversationMessageDto ToMessageDto(ConversationMessage m) => new(
            m.Id, m.ConversationId, m.Role, m.Content, m.CreatedAt);
    }
}

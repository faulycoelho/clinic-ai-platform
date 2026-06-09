using static Clinic.Domain.Enums.ConversationEnums;

namespace Clinic.Domain
{

    public class ConversationMessage
    {
        public int Id { get; private set; }
        public int ConversationId { get; private set; }
        public MessageRole Role { get; private set; }
        public string Content { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; }

        private ConversationMessage() { }

        public static ConversationMessage Create(
            int conversationId, MessageRole role, string content,
            string? externalMessageId = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(content);

            return new ConversationMessage
            {
                ConversationId = conversationId,
                Role = role,
                Content = content.Trim(),
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}

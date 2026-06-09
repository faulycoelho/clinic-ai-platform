using static Clinic.Domain.Enums.ConversationEnums;

namespace Clinic.Domain
{
    public class Conversation
    {
        private readonly List<ConversationMessage> _messages = [];

        public int Id { get; private set; }
        public string ContactName { get; private set; }
        public string ContactPhone { get; private set; }
        public ConversationStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public DateTime? ClosedAt { get; private set; }

        public IReadOnlyList<ConversationMessage> Messages => _messages.AsReadOnly();

        private Conversation() { }

        public static Conversation Create(string contactName, string contactPhone)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(contactName);
            ArgumentException.ThrowIfNullOrWhiteSpace(contactPhone);

            return new Conversation
            {
                ContactName = contactName.Trim(),
                ContactPhone = contactPhone.Trim(),
                Status = ConversationStatus.Active,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void AddMessage(ConversationMessage message)
        {
            if (Status != ConversationStatus.Active)
                throw new InvalidOperationException($"Cannot add messages to a {Status} conversation.");

            _messages.Add(message);
            UpdatedAt = DateTime.UtcNow;
        }

        public void Close()
        {
            if (Status != ConversationStatus.Active)
                throw new InvalidOperationException($"Cannot close a {Status} conversation.");

            Status = ConversationStatus.Closed;
            ClosedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}

namespace Clinic.Domain.Enums
{
    public class ConversationEnums
    {
        public enum ConversationStatus
        {
            Active = 0,
            Closed = 1,
        }
        public enum MessageRole
        {
            User = 0,
            Assistant = 1,
            System = 2,
        }
    }
}

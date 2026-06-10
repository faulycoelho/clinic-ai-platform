using Clinic.Application.Interfaces;

namespace Clinic.Application.Services
{
    public class ConversationToolRegistry(IEnumerable<IConversationTool> tools)
    {
        private readonly Dictionary<string, IConversationTool> _tools =
            tools.ToDictionary(t => t.Name, StringComparer.OrdinalIgnoreCase);

        public IReadOnlyList<IConversationTool> GetAllTools() => [.. _tools.Values];

        public IConversationTool? GetTool(string name)
            => _tools.GetValueOrDefault(name);
    }
}

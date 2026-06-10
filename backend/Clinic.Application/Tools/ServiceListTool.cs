using Clinic.Application.DTOs;
using Clinic.Application.Interfaces;
using System.Text.Json;

namespace Clinic.Application.Tools
{
    public sealed class ServiceListTools(IServiceService service) : IConversationTool
    {
        public string Name => "list_services";

        public string Description => "Get all clinic services available, including price, duration.";

        public string ParametersJsonSchema => """
        {
          "type": "object",
          "properties": {},
          "required": []
        }
        """;

        public async Task<string> ExecuteAsync(string argumentsJson, ConversationContextDto? context = null, CancellationToken ct = default)
        {
            var services = await service.GetAllAsync(ct);
            return JsonSerializer.Serialize(services);
        }
    }
}

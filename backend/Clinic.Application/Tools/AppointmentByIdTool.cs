using Clinic.Application.DTOs;
using Clinic.Application.Interfaces;
using System.Text.Json;

namespace Clinic.Application.Tools
{
    public sealed class AppointmentByIdTool(IAppointmentService service) : IConversationTool
    {
        public string Name => "get_appointment_by_id";

        public string Description => "Retrieves the details of a specific appointment by its unique ID.";

        public string ParametersJsonSchema => """
        {
          "type": "object",
          "properties": {
            "id": {
              "type": "integer",
              "description": "The unique ID of the appointment."
            }
          },
          "required": ["id"]
        }
        """;

        public async Task<string> ExecuteAsync(string argumentsJson, ConversationContextDto? context = null, CancellationToken ct = default)
        {
            using var doc = JsonDocument.Parse(argumentsJson);
            if (!doc.RootElement.TryGetProperty("id", out var idProp) || !idProp.TryGetInt32(out var id))
            {
                throw new ArgumentException("The 'id' parameter is required and must be an integer.");
            }

            var appointment = await service.GetByIdAsync(id, ct);
            return JsonSerializer.Serialize(appointment);
        }
    }
}

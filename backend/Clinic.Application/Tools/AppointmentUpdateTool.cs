using Clinic.Application.DTOs;
using Clinic.Application.Interfaces;
using System.Text.Json;

namespace Clinic.Application.Tools
{
    public sealed class AppointmentUpdateTool(IAppointmentService service) : IConversationTool
    {
        public string Name => "update_appointment";

        public string Description => "Updates an existing appointment's details using its unique ID and the new data provided.";

        public string ParametersJsonSchema => """
        {
          "type": "object",
          "properties": {
            "id": { "type": "integer", "description": "The unique ID of the appointment to update." },
            "clientName": { "type": "string", "description": "The updated name of the client." },
            "clientPhone": { "type": "string", "description": "The updated phone number." },
            "start": { "type": "string", "format": "date-time", "description": "The updated start date and time." },
            "end": { "type": "string", "format": "date-time", "description": "The updated end date and time." },
            "notes": { "type": "string", "description": "Updated notes or comments." }
          },
          "required": ["id", "clientName", "clientPhone", "start", "end"]
        }
        """;

        public async Task<string> ExecuteAsync(string argumentsJson, ConversationContextDto? context = null, CancellationToken ct = default)
        {
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            using var doc = JsonDocument.Parse(argumentsJson);
            if (!doc.RootElement.TryGetProperty("id", out var idProp) || !idProp.TryGetInt32(out var id))
            {
                throw new ArgumentException("The 'id' parameter is required.");
            }

            var dto = JsonSerializer.Deserialize<UpdateAppointmentDto>(argumentsJson, options);
            if (dto == null)
            {
                throw new ArgumentException("Invalid arguments for updating the appointment.");
            }

            var updatedAppointment = await service.UpdateAsync(id, dto, ct);
            return JsonSerializer.Serialize(updatedAppointment);
        }
    }
}

using Clinic.Application.DTOs;
using Clinic.Application.Interfaces;
using System.Text.Json;

namespace Clinic.Application.Tools
{
    public sealed class AppointmentCreateTool(IAppointmentService service) : IConversationTool
    {
        public string Name => "create_appointment";

        public string Description => "Creates a new appointment with the provided details such as patient, date, and service.";

        public string ParametersJsonSchema => """
        {
          "type": "object",
          "properties": {
            "serviceId": { "type": "integer", "description": "The ID of the service being booked." },
            "clientName": { "type": "string", "description": "The name of the client/patient." },
            "clientPhone": { "type": "string", "description": "The contact phone number of the client." },
            "start": { "type": "string", "format": "date-time", "description": "The start date and time of the appointment (ISO 8601)." },
            "end": { "type": "string", "format": "date-time", "description": "The end date and time of the appointment (ISO 8601)." },
            "notes": { "type": "string", "description": "Optional notes or details about the appointment." }
          },
          "required": ["serviceId", "clientName", "clientPhone", "start", "end"]
        }
        """;

        public async Task<string> ExecuteAsync(string argumentsJson, ConversationContextDto? context = null, CancellationToken ct = default)
        {
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var dto = JsonSerializer.Deserialize<CreateAppointmentDto>(argumentsJson, options);

            if (dto == null)
            {
                throw new ArgumentException("Invalid arguments for creating an appointment.");
            }

            var newAppointment = await service.CreateAsync(dto, ct);
            return JsonSerializer.Serialize(newAppointment);
        }
    }
}

using Clinic.Application.DTOs;
using Clinic.Application.Interfaces;
using System.Text.Json;

namespace Clinic.Application.Tools
{
    public sealed class AppointmentListTool(IAppointmentService service) : IConversationTool
    {
        public string Name => "list_appointments";

        public string Description => "Retrieves a list of all scheduled appointments.";

        public string ParametersJsonSchema =>
               """{"type":"object","properties":{},"required":[]}""";
        public async Task<string> ExecuteAsync(string argumentsJson, ConversationContextDto? context = null, CancellationToken ct = default)
        {
            var services = await service.GetAllAsync(ct);
            return JsonSerializer.Serialize(services);
        }
    }
}

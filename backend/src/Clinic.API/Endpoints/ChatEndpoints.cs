using Clinic.Application.DTOs;
using Clinic.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.API.Endpoints
{
    public static class ChatEndpoints
    {
        public static RouteGroupBuilder MapChat(
          this WebApplication app)
        {
            var group = app.MapGroup("/api/chat")
                .WithTags("Chat");

            group.MapPost("/", Chat);
            return group;
        }
        private static async Task<IResult> Chat(
           [FromServices] IChatService service,
           [FromBody] ChatMessageDto dto,
           CancellationToken ct)
        {
            return Results.Ok(await service.HandleMessageAsync(dto, ct));
        }
    }
}

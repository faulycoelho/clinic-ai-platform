using Clinic.Application.DTOs;
using Clinic.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.API.Endpoints
{
    public static class ConversationEndpoints
    {
        public static RouteGroupBuilder MapConversation(
           this WebApplication app)
        {
            var group = app.MapGroup("/api/conversation")
                .WithTags("Conversation");

            group.MapGet("/", GetAll);
            group.MapGet("{id:int}/", GetById);
            group.MapPost("/", Create);
            group.MapPost("{id:int}/close", Close);
            group.MapPost("{id:int}/send-message", SendMessage);
            return group;
        }
        private static async Task<IResult> GetAll(
           [FromServices] IConversationService service,
           CancellationToken ct)
        {
            return Results.Ok(await service.GetAllAsync(ct));
        }
        private static async Task<IResult> GetById(
           [FromServices] IConversationService service,
           int id,
           CancellationToken ct)
        {
            return Results.Ok(await service.GetByIdAsync(id, ct));
        }

        private static async Task<IResult> Create(
            [FromServices] IConversationService service,
            [FromBody] CreateConversationDto dto,
            CancellationToken ct)
        {
            return Results.Ok(await service.CreateAsync(dto, ct));
        }
        private static async Task<IResult> Close(
           [FromServices] IConversationService service,
            int id,
           CancellationToken ct)
        {
            return Results.Ok(await service.CloseAsync(id, ct));
        }

        private static async Task<IResult> SendMessage(
            [FromServices] IConversationService service,
            int id,
            [FromBody] SendMessageDto dto,
            CancellationToken ct)
        {
            return Results.Ok(await service.SendMessageAsync(id, dto, ct));
        }
    }
}

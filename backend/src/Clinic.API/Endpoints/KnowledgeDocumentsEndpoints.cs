using Clinic.Application.DTOs;
using Clinic.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.API.Endpoints
{
    public static class KnowledgeDocumentsEndpoints
    {
        public static RouteGroupBuilder MapKnowledgeDocuments(
            this WebApplication app)
        {
            var group = app.MapGroup("/api/knowledge-documents")
                .WithTags("Knowledge Documents");

            group.MapGet("/", GetAll);
            group.MapGet("{id:int}", GetById);
            group.MapPost("/", Upload)
                .DisableAntiforgery()
                .Accepts<IFormFile>("multipart/form-data");
            group.MapPut("{id:int}", Update);
            group.MapDelete("{id:int}", Delete);

            return group;
        }

        private static async Task<IResult> GetAll(
            [FromServices] IKnowledgeDocumentService service,
            CancellationToken ct)
        {
            return Results.Ok(await service.GetAllAsync(ct));
        }

        private static async Task<IResult> GetById(
            [FromServices] IKnowledgeDocumentService service,
            int id,
            CancellationToken ct)
        {
            return Results.Ok(await service.GetByIdAsync(id, ct));
        }

        static async Task<IResult> Upload(
            [FromQuery] string title,
            [FromQuery] string? description,
            [FromForm] IFormFile file,
            [FromServices] IKnowledgeDocumentService service,
            CancellationToken ct)
        {
            if (file.Length == 0)
                return Results.BadRequest("File is empty.");

            using var stream = file.OpenReadStream();
            var doc = await service.UploadAsync(
                title, description, file.FileName, file.ContentType,
                stream, file.Length, ct);

            return Results.Created(
                $"/api/knowledge-documents/{doc.Id}",
                doc);
        }

        private static async Task<IResult> Update(
            [FromServices] IKnowledgeDocumentService service,
            int id,
            [FromBody] UpdateKnowledgeDocumentDto dto,
            CancellationToken ct)
        {
            return Results.Ok(await service.UpdateAsync(id, dto, ct));
        }

        private static async Task<IResult> Delete(
            [FromServices] IKnowledgeDocumentService service, 
            int id,
            CancellationToken ct)
        {
            await service.DeleteAsync(id, ct);
            return Results.NoContent();
        }
    }
}

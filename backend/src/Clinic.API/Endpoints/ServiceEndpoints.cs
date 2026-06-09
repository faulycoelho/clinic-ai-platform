using Clinic.Application.DTOs;
using Clinic.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.API.Endpoints
{
    public static class ServiceEndpoints
    {
        public static RouteGroupBuilder MapService(
          this WebApplication app)
        {
            var group = app.MapGroup("/api/service")
                .WithTags("Service");

            group.MapGet("/", GetAll);
            group.MapGet("{id:int}/", GetById);
            group.MapPost("/", Create);
            group.MapPut("{id:int}", Update);
            group.MapDelete("{id:int}", Delete);
            return group;
        }
        private static async Task<IResult> GetAll(
           [FromServices] IServiceService service,
           CancellationToken ct)
        {
            return Results.Ok(await service.GetAllAsync(ct));
        }
        private static async Task<IResult> GetById(
           [FromServices] IServiceService service,
           int id,
           CancellationToken ct)
        {
            return Results.Ok(await service.GetByIdAsync(id, ct));
        }

        private static async Task<IResult> Create(
            [FromServices] IServiceService service,
            [FromBody] CreateServiceDto dto,
            CancellationToken ct)
        {
            return Results.Ok(await service.CreateAsync(dto, ct));
        }

        private static async Task<IResult> Update(
            [FromServices] IServiceService service,
            int id,
            [FromBody] UpdateServiceDto dto,
            CancellationToken ct)
        {
            return Results.Ok(await service.UpdateAsync(id, dto, ct));
        }

        private static async Task<IResult> Delete(
         [FromServices] IServiceService service,
         int id,
         CancellationToken ct)
        {
            await service.DeleteAsync(id, ct);
            return Results.NoContent();
        }
    }
}

using Clinic.Application.DTOs;
using Clinic.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.API.Endpoints
{
    public static class AppointmentEndpoints
    {
        public static RouteGroupBuilder MapAppointment(
           this WebApplication app)
        {
            var group = app.MapGroup("/api/appointment")
                .WithTags("Appointment");

            group.MapGet("/", GetAll);
            group.MapGet("{id:int}/", GetById);
            group.MapPost("/", Create);
            group.MapPut("{id:int}", Update);
            group.MapPatch("{id:int}/status", ChangeStatus);
            return group;
        }
        private static async Task<IResult> GetAll(
           [FromServices] IAppointmentService service,
           CancellationToken ct)
        {
            return Results.Ok(await service.GetAllAsync(ct));
        }
        private static async Task<IResult> GetById(
           [FromServices] IAppointmentService service,
           int id,
           CancellationToken ct)
        {
            return Results.Ok(await service.GetByIdAsync(id, ct));
        }

        private static async Task<IResult> Create(
            [FromServices] IAppointmentService service,
            [FromBody] CreateAppointmentDto dto,
            CancellationToken ct)
        {
            return Results.Ok(await service.CreateAsync(dto, ct));
        }

        private static async Task<IResult> Update(
            [FromServices] IAppointmentService service,
            int id,
            [FromBody] UpdateAppointmentDto dto,
            CancellationToken ct)
        {
            return Results.Ok(await service.UpdateAsync(id, dto, ct));
        }

        private static async Task<IResult> ChangeStatus(
         [FromServices] IAppointmentService service,
         int id,
         [FromBody] ChangeAppointmentStatusDto dto,
         CancellationToken ct)
        {
            return Results.Ok(await service.ChangeStatusAsync(id, dto, ct));
        }
    }
}

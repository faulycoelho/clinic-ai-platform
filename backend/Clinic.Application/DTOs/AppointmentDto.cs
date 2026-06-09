using Clinic.Domain.Enums;

namespace Clinic.Application.DTOs
{

    public record AppointmentDto(
        int Id,
        int ServiceId,
        string ServiceName,
        string ClientName,
        string ClientPhone,
        DateTimeOffset Start,
        DateTimeOffset End,
        AppointmentStatus Status,
        string? Notes,
        DateTime CreatedAt,
        DateTime? UpdatedAt);

    public record CreateAppointmentDto(
        int ServiceId,
        string ClientName,
        string ClientPhone,
        DateTimeOffset Start,
        DateTimeOffset End,
        string? Notes = null);

    public record UpdateAppointmentDto(
        string ClientName,
        string ClientPhone,
        string? Notes,
        DateTimeOffset Start,
        DateTimeOffset End);

    public record ChangeAppointmentStatusDto(
        AppointmentStatus NewStatus);

}

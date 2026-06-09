using Clinic.Application.DTOs;

namespace Clinic.Application.Interfaces
{
    public interface IAppointmentService
    {
        Task<IReadOnlyList<AppointmentDto>> GetAllAsync(CancellationToken ct = default);
        Task<AppointmentDto> GetByIdAsync(int id, CancellationToken ct = default);
        Task<AppointmentDto> CreateAsync(CreateAppointmentDto dto, CancellationToken ct = default);
        Task<AppointmentDto> UpdateAsync(int id, UpdateAppointmentDto dto, CancellationToken ct = default);
        Task<AppointmentDto> ChangeStatusAsync(int id, ChangeAppointmentStatusDto dto, CancellationToken ct = default);
    }
}

using Clinic.Domain;

namespace Clinic.Application.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<Appointment?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<Appointment>> GetAllAsync(CancellationToken ct = default);
        Task<Appointment> AddAsync(Appointment appointment, CancellationToken ct = default);
        Task<Appointment> UpdateAsync(Appointment appointment, CancellationToken ct = default);
    }
}

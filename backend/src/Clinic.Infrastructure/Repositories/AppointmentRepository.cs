using Clinic.Application.Interfaces;
using Clinic.Domain;
using Clinic.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Infrastructure.Repositories
{
    public class AppointmentRepository(AppDbContext db) : IAppointmentRepository
    {
        public async Task<Appointment?> GetByIdAsync(int id, CancellationToken ct = default)
            => await db.Appointments.FindAsync([id], ct);

        public async Task<IReadOnlyList<Appointment>> GetAllAsync(CancellationToken ct = default)
            => await db.Appointments.AsNoTracking().OrderByDescending(a => a.Start).ToListAsync(ct);
        public async Task<Appointment> AddAsync(Appointment appointment, CancellationToken ct = default)
        {
            db.Appointments.Add(appointment);
            await db.SaveChangesAsync(ct);
            return appointment;
        }

        public async Task<Appointment> UpdateAsync(Appointment appointment, CancellationToken ct = default)
        {
            db.Appointments.Update(appointment);
            await db.SaveChangesAsync(ct);
            return appointment;
        }
    }
}

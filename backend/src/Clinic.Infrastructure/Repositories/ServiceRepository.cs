using Clinic.Application.Interfaces;
using Clinic.Domain;
using Clinic.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Infrastructure.Repositories
{
    public class ServiceRepository(AppDbContext db) : IServiceRepository
    {
        public async Task<Service?> GetByIdAsync(int id, CancellationToken ct = default)
            => await db.Services.FindAsync([id], ct);

        public async Task<IReadOnlyList<Service>> GetAllAsync(CancellationToken ct = default)
            => await db.Services.AsNoTracking().OrderBy(s => s.Name).ToListAsync(ct);

        public async Task<Service> AddAsync(Service service, CancellationToken ct = default)
        {
            db.Services.Add(service);
            await db.SaveChangesAsync(ct);
            return service;
        }

        public async Task AddRangeAsync(Service[] services, CancellationToken ct = default)
        {
            db.Services.AddRange(services);
            await db.SaveChangesAsync(ct); 
        }

        public async Task<Service> UpdateAsync(Service service, CancellationToken ct = default)
        {
            db.Services.Update(service);
            await db.SaveChangesAsync(ct);
            return service;
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var service = await db.Services.FindAsync([id], ct);
            if (service is not null)
            {
                db.Services.Remove(service);
                await db.SaveChangesAsync(ct);
            }
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
            => await db.Services.AnyAsync(s => s.Id == id, ct);
    }

}

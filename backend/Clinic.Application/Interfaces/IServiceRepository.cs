using Clinic.Domain;

namespace Clinic.Application.Interfaces
{
    public interface IServiceRepository
    {
        Task<Service?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<Service>> GetAllAsync(CancellationToken ct = default);
        Task<Service> AddAsync(Service service, CancellationToken ct = default);
        Task AddRangeAsync(Service[] services, CancellationToken ct = default);
        Task<Service> UpdateAsync(Service service, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    }
}

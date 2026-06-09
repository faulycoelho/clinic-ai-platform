using Clinic.Application.DTOs;

namespace Clinic.Application.Interfaces
{
    public interface IServiceService
    {
        Task<IReadOnlyList<ServiceDto>> GetAllAsync(CancellationToken ct = default);
        Task<ServiceDto> GetByIdAsync(int id, CancellationToken ct = default);
        Task<ServiceDto> CreateAsync(CreateServiceDto dto, CancellationToken ct = default);
        Task<ServiceDto> UpdateAsync(int id, UpdateServiceDto dto, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}

using Clinic.Application.DTOs;
using Clinic.Application.Interfaces;
using Clinic.Domain;

namespace Clinic.Application.Services
{

    public class ServiceService(IServiceRepository repo) : IServiceService
    {
        public async Task<IReadOnlyList<ServiceDto>> GetAllAsync(CancellationToken ct = default)
        {
            var services = await repo.GetAllAsync(ct);
            return services.Select(s => ToDto(s)).ToList();
        }

        public async Task<ServiceDto> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var service = await repo.GetByIdAsync(id, ct)
                ?? throw new KeyNotFoundException($"Service {id} not found.");
            return ToDto(service);
        }

        public async Task<ServiceDto> CreateAsync(CreateServiceDto dto, CancellationToken ct = default)
        {
            var service = Service.Create(dto.Name, dto.DurationMinutes,
                dto.Price, dto.Currency, dto.Description);
            var saved = await repo.AddAsync(service, ct);
            return ToDto(saved);
        }

        public async Task<ServiceDto> UpdateAsync(int id, UpdateServiceDto dto, CancellationToken ct = default)
        {
            var service = await repo.GetByIdAsync(id, ct)
                ?? throw new KeyNotFoundException($"Service {id} not found.");

            service.Update(dto.Name, dto.DurationMinutes,
                dto.Price, dto.Currency, dto.Description, dto.IsActive);
            var saved = await repo.UpdateAsync(service, ct);
            return ToDto(saved);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            _ = await repo.GetByIdAsync(id, ct)
                ?? throw new KeyNotFoundException($"Service {id} not found.");
            await repo.DeleteAsync(id, ct);
        }

        private static ServiceDto ToDto(Service s) => new(
            s.Id, s.Name, s.Description,
            s.DurationMinutes, s.Price, s.Currency, s.IsActive,
            s.CreatedAt, s.UpdatedAt);
    }

}

using Clinic.Application.DTOs;
using Clinic.Application.Interfaces;
using Clinic.Domain;

namespace Clinic.Application.Services
{

    public class AppointmentService(
     IAppointmentRepository repo,
     IServiceRepository serviceRepo) : IAppointmentService
    {
        public async Task<IReadOnlyList<AppointmentDto>> GetAllAsync(
            CancellationToken ct = default)
        {
            var appointments = await repo.GetAllAsync(ct);

            var services = (await serviceRepo.GetAllAsync(ct))
                .ToDictionary(s => s.Id, s => s.Name);

            return appointments
                .Select(a => ToDto(a, services[a.ServiceId]))
                .ToList();
        }

        public async Task<AppointmentDto> GetByIdAsync(
            int id,
            CancellationToken ct = default)
        {
            var appointment = await GetAppointmentAsync(id, ct);
            var service = await GetServiceAsync(appointment.ServiceId, ct);

            return ToDto(appointment, service.Name);
        }

        public async Task<AppointmentDto> CreateAsync(
            CreateAppointmentDto dto,
            CancellationToken ct = default)
        {
            var service = await GetServiceAsync(dto.ServiceId, ct);

            var appointment = Appointment.Create(
                dto.ServiceId,
                dto.ClientName,
                dto.ClientPhone,
                dto.Start,
                dto.End,
                dto.Notes);

            var saved = await repo.AddAsync(appointment, ct);

            return ToDto(saved, service.Name);
        }

        public async Task<AppointmentDto> UpdateAsync(
            int id,
            UpdateAppointmentDto dto,
            CancellationToken ct = default)
        {
            var appointment = await GetAppointmentAsync(id, ct);
            var service = await GetServiceAsync(appointment.ServiceId, ct);

            appointment.Update(
                dto.ClientName,
                dto.ClientPhone,
                dto.Notes,
                dto.Start,
                dto.End);

            var saved = await repo.UpdateAsync(appointment, ct);

            return ToDto(saved, service.Name);
        }

        public async Task<AppointmentDto> ChangeStatusAsync(
            int id,
            ChangeAppointmentStatusDto dto,
            CancellationToken ct = default)
        {
            var appointment = await GetAppointmentAsync(id, ct);
            var service = await GetServiceAsync(appointment.ServiceId, ct);

            appointment.SetStatus(dto.NewStatus);

            var saved = await repo.UpdateAsync(appointment, ct);

            return ToDto(saved, service.Name);
        }

        private async Task<Appointment> GetAppointmentAsync(
            int id,
            CancellationToken ct)
        {
            return await repo.GetByIdAsync(id, ct)
                ?? throw new KeyNotFoundException($"Appointment {id} not found.");
        }

        private async Task<Service> GetServiceAsync(
            int serviceId,
            CancellationToken ct)
        {
            return await serviceRepo.GetByIdAsync(serviceId, ct)
                ?? throw new KeyNotFoundException($"Service {serviceId} not found.");
        }

        private static AppointmentDto ToDto(
            Appointment appointment,
            string serviceName)
        {
            return new AppointmentDto(
                appointment.Id,
                appointment.ServiceId,
                serviceName,
                appointment.ClientName,
                appointment.ClientPhone,
                appointment.Start,
                appointment.End,
                appointment.Status,
                appointment.Notes,
                appointment.CreatedAt,
                appointment.UpdatedAt);
        }
    }

}

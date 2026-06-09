using Clinic.Domain.Enums;

namespace Clinic.Domain
{
    public class Appointment
    {
        public int Id { get; private set; }
        public int ServiceId { get; private set; }
        public string ClientName { get; private set; }
        public string ClientPhone { get; private set; }
        public DateTimeOffset Start { get; private set; }
        public DateTimeOffset End { get; private set; }
        public AppointmentStatus Status { get; private set; }
        public string? Notes { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        private static readonly Dictionary<AppointmentStatus, AppointmentStatus[]> ValidTransitions = new()
        {
            [AppointmentStatus.Scheduled] = [AppointmentStatus.Confirmed, AppointmentStatus.Cancelled],
            [AppointmentStatus.Confirmed] = [AppointmentStatus.InProgress, AppointmentStatus.Cancelled, AppointmentStatus.NoShow],
            [AppointmentStatus.InProgress] = [AppointmentStatus.Completed, AppointmentStatus.Cancelled],
            [AppointmentStatus.Completed] = [],
            [AppointmentStatus.Cancelled] = [],
            [AppointmentStatus.NoShow] = []
        };

        private Appointment() { }

        public static Appointment Create(
            int serviceId,
            string clientName, string clientPhone,
            DateTimeOffset start, DateTimeOffset end,
            string? notes = null)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(serviceId);
            ArgumentException.ThrowIfNullOrWhiteSpace(clientName);
            if (end <= start)
                throw new ArgumentException("End must be after Start.");

            return new Appointment
            {
                ServiceId = serviceId,
                ClientName = clientName.Trim(),
                ClientPhone = clientPhone.Trim(),
                Start = start,
                End = end,
                Status = AppointmentStatus.Scheduled,
                Notes = notes?.Trim(),
                CreatedAt = DateTime.UtcNow
            };
        }

        public void SetStatus(AppointmentStatus newStatus)
        {
            if (!ValidTransitions.TryGetValue(Status, out var allowed) || !allowed.Contains(newStatus))
                throw new InvalidOperationException(
                    $"Cannot transition from {Status} to {newStatus}.");

            Status = newStatus;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Update(
            string clientName, string clientPhone, string? notes,
            DateTimeOffset start, DateTimeOffset end)
        {
            if (Status != AppointmentStatus.Scheduled && Status != AppointmentStatus.Confirmed)
                throw new InvalidOperationException(
                    $"Cannot update appointment in {Status} status.");

            ArgumentException.ThrowIfNullOrWhiteSpace(clientName);
            if (end <= start)
                throw new ArgumentException("End must be after Start.");

            ClientName = clientName.Trim();
            ClientPhone = clientPhone.Trim();
            Notes = notes?.Trim();
            Start = start;
            End = end;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}

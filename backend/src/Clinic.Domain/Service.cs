namespace Clinic.Domain
{
    public class Service
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; }
        public int DurationMinutes { get; private set; }
        public decimal Price { get; private set; }
        public string Currency { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        private Service() { }

        public static Service Create(
            string name, int durationMinutes, decimal price,
            string currency, string description)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentException.ThrowIfNullOrWhiteSpace(description);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(durationMinutes);
            ArgumentOutOfRangeException.ThrowIfNegative(price);
            ArgumentException.ThrowIfNullOrWhiteSpace(currency);

            return new Service
            {
                Name = name.Trim(),
                Description = description.Trim(),
                DurationMinutes = durationMinutes,
                Price = price,
                Currency = currency.Trim().ToUpperInvariant(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void Update(
            string name, int durationMinutes, decimal price,
            string currency, string description, bool isActive)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(durationMinutes);
            ArgumentOutOfRangeException.ThrowIfNegative(price);
            ArgumentException.ThrowIfNullOrWhiteSpace(currency);

            Name = name.Trim();
            Description = description.Trim();
            DurationMinutes = durationMinutes;
            Price = price;
            Currency = currency.Trim().ToUpperInvariant();
            IsActive = isActive;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}

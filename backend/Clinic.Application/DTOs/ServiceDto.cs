namespace Clinic.Application.DTOs
{
    public record ServiceDto(
        int Id,
        string Name,
        string Description,
        int DurationMinutes,
        decimal Price,
        string Currency,
        bool IsActive,
        DateTime CreatedAt,
        DateTime? UpdatedAt);

    public record CreateServiceDto(
        string Name,
        int DurationMinutes,
        decimal Price,
        string Currency,
        string Description);

    public record UpdateServiceDto(
        string Name,
        int DurationMinutes,
        decimal Price,
        string Currency,
        string Description,
        bool IsActive);
}

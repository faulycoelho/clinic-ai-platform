using Clinic.Application.Interfaces;
using Clinic.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Clinic.Infrastructure.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var repository = services.GetRequiredService<IServiceRepository>();

            var existing = await repository.GetAllAsync();

            if (existing.Any())
                return;

            var servicesList = new[]
               {
                Service.Create(
                    name: "General Consultation",
                    durationMinutes: 30,
                    price: 100m,
                    currency: "USD",
                    description: "Comprehensive medical consultation for diagnosis, treatment planning, and health assessment."
                ),

                Service.Create(
                    name: "Follow-up Consultation",
                    durationMinutes: 20,
                    price: 60m,
                    currency: "USD",
                    description: "Review of treatment progress, test results, and ongoing medical care."
                ),

                Service.Create(
                    name: "Annual Health Checkup",
                    durationMinutes: 60,
                    price: 180m,
                    currency: "USD",
                    description: "Preventive health examination including medical history review and general assessment."
                ),

                Service.Create(
                    name: "Vaccination Appointment",
                    durationMinutes: 15,
                    price: 40m,
                    currency: "USD",
                    description: "Administration of recommended vaccines with patient evaluation and guidance."
                ),

                Service.Create(
                    name: "Laboratory Test Collection",
                    durationMinutes: 15,
                    price: 30m,
                    currency: "USD",
                    description: "Sample collection for laboratory analysis, including blood and other diagnostic tests."
                )
            };

            await repository.AddRangeAsync(servicesList);
        }
    }
}

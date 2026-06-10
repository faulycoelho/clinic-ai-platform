using Clinic.Application.Interfaces;
using Clinic.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Clinic.Infrastructure.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            await SeedServices(services);
            await SeedDocs(services);
        }
        private static async Task SeedDocs(IServiceProvider services)
        {
            var docsPath = ResolveDocsPath();
            if (string.IsNullOrWhiteSpace(docsPath))
                return;

            var knowledgeDocumentRepository = services.GetRequiredService<IKnowledgeDocumentRepository>();
            var hasAny = await knowledgeDocumentRepository.HasAnyAsync();
            if (hasAny)
            {
                return;
            }

            var files = Directory.GetFiles(docsPath, "*.pdf", SearchOption.TopDirectoryOnly);
            if (files.Length == 0)
                return;

            var knowledgeDocumentService = services.GetRequiredService<IKnowledgeDocumentService>();
            foreach (var filePath in files.OrderBy(f => f))
            {
                var fileInfo = new FileInfo(filePath);

                await using var stream = File.OpenRead(filePath);

                await knowledgeDocumentService.UploadAsync(
                    title: Path.GetFileNameWithoutExtension(filePath),
                    description: "Seed document",
                    fileName: fileInfo.Name,
                    contentType: "application/pdf",
                    fileStream: stream,
                    fileSize: fileInfo.Length);
            }
        }

        private static string? ResolveDocsPath()
        {
            var fromCwd = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "docs"));
            if (Directory.Exists(fromCwd))
                return fromCwd;

            return null;
        }

        private static async Task SeedServices(IServiceProvider services)
        {
            var repository = services.GetRequiredService<IServiceRepository>();
            var hasAny = await repository.HasAnyAsync();
            if (hasAny)
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

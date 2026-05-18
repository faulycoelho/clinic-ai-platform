using Clinic.Application.Interfaces;

namespace Clinic.Infrastructure.TextExtraction
{
    public class PlainTextExtractor : ITextExtractor
    {
        private static readonly HashSet<string> SupportedTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "text/plain",
            "text/markdown",
            "text/csv"
        };

        public bool CanExtract(string contentType) => SupportedTypes.Contains(contentType);

        public async Task<string?> ExtractTextAsync(Stream content, CancellationToken ct = default)
        {
            using var reader = new StreamReader(content, leaveOpen: true);
            return await reader.ReadToEndAsync(ct);
        }
    }
}
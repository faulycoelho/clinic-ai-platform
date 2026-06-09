using Clinic.Application.Interfaces;
using System.Text;
using UglyToad.PdfPig;

namespace Clinic.Infrastructure.TextExtraction
{
    public class PdfTextExtractor : ITextExtractor
    {
        public bool CanExtract(string contentType)
            => contentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase);

        public Task<string?> ExtractTextAsync(Stream content, CancellationToken ct = default)
        {
            var sb = new StringBuilder();

            using var document = PdfDocument.Open(content);
            foreach (var page in document.GetPages())
            {
                sb.AppendLine(page.Text);
            }

            var text = sb.ToString().Trim();
            return Task.FromResult<string?>(string.IsNullOrWhiteSpace(text) ? null : text);
        }
    }
}
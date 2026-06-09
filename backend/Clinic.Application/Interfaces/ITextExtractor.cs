using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Application.Interfaces
{
    public interface ITextExtractor
    {
        bool CanExtract(string contentType);
        Task<string?> ExtractTextAsync(Stream content, CancellationToken ct = default);
    }
}

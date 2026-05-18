using Clinic.Application.DTOs;
using Clinic.Application.Interfaces;
using Clinic.Domain;

namespace Clinic.Application.Services
{
    public class KnowledgeDocumentService(
        IKnowledgeDocumentRepository repository,
        IKnowledgeDocumentChunkRepository repositoryChunk,
        IStorageService storageService,
        ChunkService chunkService,
        ILLMProvider  lLM,
        IEnumerable<ITextExtractor> textExtractors) : IKnowledgeDocumentService
    {
        private const string BucketName = "clinic-knowledge";

        public async Task<IReadOnlyList<KnowledgeDocumentDto>> GetAllAsync(CancellationToken ct = default)
        {
            var docs = await repository.GetAllActiveAsync(ct);
            return docs.Select(ToDto).ToList();
        }

        public async Task<KnowledgeDocumentDto> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var doc = await repository.GetByIdAsync(id, ct)
                ?? throw new KeyNotFoundException($"KnowledgeDocument {id} not found.");
            return ToDto(doc);
        }

        public async Task<KnowledgeDocumentDto> UploadAsync(
            string title, string? description, string fileName,
            string contentType, Stream fileStream, long fileSize,
            CancellationToken ct = default)
        {
            var storageKey = $"knowledge-documents/{Guid.NewGuid()}/{fileName}";

            await storageService.EnsureBucketExistsAsync(BucketName, ct);
            await storageService.UploadAsync(BucketName, storageKey, fileStream, contentType, ct);

            var doc = KnowledgeDocument.Create(title, fileName, contentType, BucketName, storageKey, fileSize, description);

            await repository.AddAsync(doc, ct);


            //ToDo: Background Service
            try
            {
                doc.MarkProcessing();
                await repository.UpdateAsync(doc, ct);

                using var readStream = await storageService.DownloadAsync(BucketName, storageKey, ct);

                var extractor = textExtractors.FirstOrDefault(e => e.CanExtract(contentType));
                if (extractor is null)
                {
                    throw new KeyNotFoundException();
                }

                fileStream.Position = 0;
                var text = await extractor.ExtractTextAsync(fileStream, ct);

                ArgumentException.ThrowIfNullOrWhiteSpace(text);

                var chunks = chunkService.Split(text);

                var tasks = chunks.Select(async (chunk, index) =>
                {
                    var embedding = await lLM.GenerateEmbeddingAsync(
                        chunk,
                        ct);

                    var entity = KnowledgeDocumentChunk.Create(doc.Id, index, chunk, embedding);
                    await repositoryChunk.AddAsync(entity, ct);
                });

                await Task.WhenAll(tasks);

                doc.MarkCompleted(chunks.Count);
            }
            catch (Exception ex)
            {
                doc.MarkFailed(ex.Message);
            }

            await repository.UpdateAsync(doc, ct);
            return ToDto(doc);
        }

        public async Task<KnowledgeDocumentDto> UpdateAsync(int id, UpdateKnowledgeDocumentDto dto, CancellationToken ct = default)
        {
            var doc = await repository.GetByIdAsync(id, ct)
                ?? throw new KeyNotFoundException($"KnowledgeDocument {id} not found.");

            doc.Update(dto.Title, dto.Description, dto.IsActive);
            await repository.UpdateAsync(doc, ct);
            return ToDto(doc);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            _ = await repository.GetByIdAsync(id, ct)
                ?? throw new KeyNotFoundException($"KnowledgeDocument {id} not found.");
            await repository.DeleteAsync(id, ct);
        }

        private static KnowledgeDocumentDto ToDto(KnowledgeDocument doc) => new(
            doc.Id, doc.Title, doc.Description, doc.FileName,
            doc.ContentType, doc.FileSizeBytes, doc.IsActive,
            doc.CreatedAt, doc.UpdatedAt);
    }
}

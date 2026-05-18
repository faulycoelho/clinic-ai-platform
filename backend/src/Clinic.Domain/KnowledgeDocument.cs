using Clinic.Domain.Enums;

namespace Clinic.Domain
{
    public class KnowledgeDocument
    {
        public int Id { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public string FileName { get; private set; } = string.Empty;
        public string ContentType { get; private set; } = string.Empty;
        public string BucketKey { get; private set; } = string.Empty;
        public string StorageKey { get; private set; } = string.Empty;
        public int? ProcessedChunks { get; set; }
        public long FileSizeBytes { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public DocumentProcessingStatus ProcessingStatus { get; private set; } = DocumentProcessingStatus.Pending;
        public string? ProcessingError { get; private set; }

        private KnowledgeDocument() { }

        public static KnowledgeDocument Create(
            string title, string fileName, string contentType,
            string bucketKey, string storageKey, long fileSizeBytes, string? description = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(title);
            ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
            ArgumentException.ThrowIfNullOrWhiteSpace(bucketKey);
            ArgumentException.ThrowIfNullOrWhiteSpace(storageKey);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(fileSizeBytes);

            return new KnowledgeDocument
            {
                Title = title.Trim(),
                FileName = fileName.Trim(),
                ContentType = contentType.Trim(),
                BucketKey = bucketKey.Trim(),
                StorageKey = storageKey.Trim(),
                FileSizeBytes = fileSizeBytes,
                Description = description?.Trim(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void Update(string title, string? description, bool isActive)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(title);
            Title = title.Trim();
            Description = description?.Trim();
            IsActive = isActive;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkProcessing() => ProcessingStatus = DocumentProcessingStatus.Processing;

        public void MarkCompleted(int chunksProcessed)
        {
            ProcessedChunks = chunksProcessed;
            ProcessingStatus = DocumentProcessingStatus.Completed;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkFailed(string error)
        {
            ProcessingError = error;
            ProcessingStatus = DocumentProcessingStatus.Failed;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}

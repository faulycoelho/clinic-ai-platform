namespace Clinic.Application.Interfaces
{
    public interface IStorageService
    {
        Task EnsureBucketExistsAsync(string bucket, CancellationToken cancellationToken = default);
        Task UploadAsync(string bucket, string key, Stream content, string contentType, CancellationToken cancellationToken = default);
        Task<string> GetPresignedUrlAsync(string bucket, string key, TimeSpan expiry, CancellationToken cancellationToken = default);
        Task<Stream> DownloadAsync(string bucket, string key, CancellationToken cancellationToken = default);
    }
}

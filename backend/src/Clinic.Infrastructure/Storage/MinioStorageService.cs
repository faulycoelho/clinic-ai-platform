using Clinic.Application.Interfaces;
using Minio;
using Minio.DataModel.Args;

namespace Clinic.Infrastructure.Storage
{
    public class MinioStorageService(IMinioClient minioClient) : IStorageService
    {
        public async Task EnsureBucketExistsAsync(string bucket, CancellationToken cancellationToken = default)
        {
            var exists = await minioClient.BucketExistsAsync(
                new BucketExistsArgs().WithBucket(bucket), cancellationToken);

            if (!exists)
                await minioClient.MakeBucketAsync(
                    new MakeBucketArgs().WithBucket(bucket), cancellationToken);
        }

        public async Task UploadAsync(string bucket, string key, Stream content, string contentType, CancellationToken cancellationToken = default)
        {
            await minioClient.PutObjectAsync(new PutObjectArgs()
                .WithBucket(bucket)
                .WithObject(key)
                .WithStreamData(content)
                .WithObjectSize(content.Length)
                .WithContentType(contentType), cancellationToken);
        }

        public async Task<string> GetPresignedUrlAsync(string bucket, string key, TimeSpan expiry, CancellationToken cancellationToken = default)
        {
            return await minioClient.PresignedGetObjectAsync(new PresignedGetObjectArgs()
                .WithBucket(bucket)
                .WithObject(key)
                .WithExpiry((int)expiry.TotalSeconds));
        }

        public async Task<Stream> DownloadAsync(string bucket, string key, CancellationToken cancellationToken = default)
        {
            var memoryStream = new MemoryStream();

            await minioClient.GetObjectAsync(
                new GetObjectArgs()
                    .WithBucket(bucket)
                    .WithObject(key)
                    .WithCallbackStream(stream =>
                    {
                        stream.CopyTo(memoryStream);
                    }),
                cancellationToken);

            memoryStream.Position = 0;

            return memoryStream;
        }
    }
}

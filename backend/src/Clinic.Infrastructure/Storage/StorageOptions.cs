namespace Clinic.Infrastructure.Storage
{
    public class StorageOptions
    {
        public string Endpoint { get; set; } = "";
        public string AccessKey { get; set; } = "";
        public string SecretKey { get; set; } = "";
        public string Bucket { get; set; } = "";
        public bool UseSSL { get; set; }
    }
}

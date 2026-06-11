using Clinic.Application.Interfaces;
using Clinic.Application.Services;
using Clinic.Infrastructure.Data;
using Clinic.Infrastructure.LLMs;
using Clinic.Infrastructure.Repositories;
using Clinic.Infrastructure.Storage;
using Clinic.Infrastructure.TextExtraction;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;

namespace Clinic.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
          this IServiceCollection services,
          IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
              options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                o => o.UseVector()
              ));

            // Repositories
            services.AddScoped<IKnowledgeDocumentRepository, KnowledgeDocumentRepository>();
            services.AddScoped<IKnowledgeDocumentChunkRepository, KnowledgeDocumentChunkRepository>();
            services.AddScoped<IConversationRepository, ConversationRepository>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();

            // LLM Provider:
            var llmSection = configuration.GetSection("LLM");
            var llmOptions = new LlmOptions
            {
                Provider = llmSection["LLM_PROVIDER"] ?? throw new InvalidDataException(),
                ApiKey = llmSection["LLM_API_KEY"] ?? throw new InvalidDataException(),
                Model = llmSection["LLM_MODEL"] ?? throw new InvalidDataException(),
                ModelEmbedding = llmSection["LLM_MODEL_EMBEDDING"] ?? throw new InvalidDataException()
            };
            services.AddSingleton(llmOptions);

            switch (llmOptions.Provider.ToLowerInvariant())
            {
                case "gemini":
                    services.AddHttpClient<GeminiLlmProvider>();
                    services.AddScoped<ILLMProvider>(sp => sp.GetRequiredService<GeminiLlmProvider>());
                    break;
                default:
                    throw new NotImplementedException();
            }

            var storageSection = configuration.GetSection("Storage");
            var storageOptions = new StorageOptions
            {
                Endpoint = storageSection["Endpoint"] ?? throw new InvalidDataException(),
                AccessKey = storageSection["AccessKey"] ?? throw new InvalidDataException(),
                SecretKey = storageSection["SecretKey"] ?? throw new InvalidDataException(),
                Bucket = storageSection["Bucket"] ?? throw new InvalidDataException(),
                UseSSL = string.Equals(storageSection["UseSSL"], "true", StringComparison.OrdinalIgnoreCase)
            };

            services.AddSingleton<IMinioClient>(_ => new MinioClient()
             .WithEndpoint(storageOptions.Endpoint)
             .WithCredentials(storageOptions.AccessKey, storageOptions.SecretKey)
             .WithSSL(storageOptions.UseSSL)
             .Build());

            services.AddScoped<IStorageService, MinioStorageService>();


            // Text extractors
            services.AddScoped<ITextExtractor, PlainTextExtractor>();
            services.AddScoped<ITextExtractor, PdfTextExtractor>();

            // Application Services
            services.AddScoped<IKnowledgeDocumentService, KnowledgeDocumentService>();
            services.AddScoped<IConversationService, ConversationService>();
            services.AddScoped<IServiceService, ServiceService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<ChunkService>();
            services.AddScoped<ConversationToolRegistry>();

            var tools = typeof(IConversationTool).Assembly
            .GetTypes()
            .Where(t =>
                !t.IsAbstract &&
                typeof(IConversationTool).IsAssignableFrom(t));

            foreach (var tool in tools)
            {
                services.AddScoped(typeof(IConversationTool), tool);
            }
            return services;
        }
    }
}
using Clinic.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinic.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<KnowledgeDocument> KnowledgeDocuments => Set<KnowledgeDocument>();
        public DbSet<KnowledgeDocumentChunk> KnowledgeDocumentChunks => Set<KnowledgeDocumentChunk>();
        public DbSet<Conversation> Conversations => Set<Conversation>();
        public DbSet<ConversationMessage> ConversationMessages => Set<ConversationMessage>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<Service> Services => Set<Service>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("vector");

            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }

    public class KnowledgeDocumentChunkConfiguration
    : IEntityTypeConfiguration<KnowledgeDocumentChunk>
    {
        public void Configure(EntityTypeBuilder<KnowledgeDocumentChunk> builder)
        {
            builder.ToTable("KnowledgeDocumentChunks");

            builder.Property(x => x.Embedding)
               .HasConversion(
                   v => new Pgvector.Vector(v),
                   v => v.ToArray())
               .HasColumnType("vector(3072)");
        }
    }
}

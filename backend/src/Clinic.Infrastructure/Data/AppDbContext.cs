using Clinic.Domain;
using Microsoft.EntityFrameworkCore;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);



            //modelBuilder.Entity<Conversation>(entity =>
            //{
            //    entity.Navigation(c => c.Messages)
            //          .UsePropertyAccessMode(PropertyAccessMode.Field);
            //});

            //modelBuilder.Entity<ConversationMessage>(entity =>
            //{
            //    entity.HasOne<Conversation>().WithMany(c => c.Messages)
            //          .HasForeignKey(m => m.ConversationId).OnDelete(DeleteBehavior.Cascade);
            //});
        }
    }
}

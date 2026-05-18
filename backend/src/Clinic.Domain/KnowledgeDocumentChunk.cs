namespace Clinic.Domain
{
    public class KnowledgeDocumentChunk
    {
        public int Id { get; private set; }
        public int KnowledgeDocumentId { get; private set; }
        public int ChunkIndex { get; private set; }
        public string Content { get; private set; } = default!;
        public float[] Embedding { get; private set; } = default!;
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        private KnowledgeDocumentChunk() { }

        public static KnowledgeDocumentChunk Create(
          int knowledgeDocumentId, int chunkIndex, string content, float[] embedding)
        {
            ArgumentNullException.ThrowIfNull(embedding);
            ArgumentException.ThrowIfNullOrWhiteSpace(content);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(knowledgeDocumentId);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(chunkIndex);

            return new KnowledgeDocumentChunk
            {
                KnowledgeDocumentId = knowledgeDocumentId,
                ChunkIndex = chunkIndex,
                Content = content,
                Embedding = embedding,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}

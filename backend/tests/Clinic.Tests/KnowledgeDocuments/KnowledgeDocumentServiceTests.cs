using Clinic.Application.DTOs;
using Clinic.Application.Interfaces;
using Clinic.Application.Services;
using Clinic.Domain;
using FluentAssertions;
using Moq;

namespace Clinic.Tests.KnowledgeDocuments
{
    public class KnowledgeDocumentServiceTests
    {
        private readonly Mock<IKnowledgeDocumentRepository> _repo = new();
        private readonly Mock<IKnowledgeDocumentChunkRepository> _repoChunk = new();
        private readonly Mock<IStorageService> _storage = new();
        private readonly Mock<ITextExtractor> _textExtractor = new();
        private readonly Mock<ILLMProvider> _llm  = new();
     
        private readonly KnowledgeDocumentService _sut;
        private readonly ChunkService _sutChunk;

        public KnowledgeDocumentServiceTests()
        {
            _textExtractor.Setup(e => e.CanExtract("text/plain")).Returns(true);
            _textExtractor.Setup(e => e.ExtractTextAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("Extracted text content");
            _sutChunk = new ChunkService();
            _sut = new KnowledgeDocumentService(
                _repo.Object, _repoChunk.Object, _storage.Object, _sutChunk, _llm.Object, new[] { _textExtractor.Object });
            
        }

        [Fact]
        public async Task GetAllAsync_ReturnsDtos()
        {
            var docs = new List<KnowledgeDocument>
        {
            KnowledgeDocument.Create("FAQ", "faq.txt", "text/plain", "bkey1", "key1", 100)
        };
            _repo.Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(docs);

            var result = await _sut.GetAllAsync();

            result.Should().HaveCount(1);
            result[0].Title.Should().Be("FAQ");
        }

        [Fact]
        public async Task GetByIdAsync_NotFound_ThrowsKeyNotFoundException()
        {
            _repo.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
                .ReturnsAsync((KnowledgeDocument?)null);

            var act = () => _sut.GetByIdAsync(99);

            await act.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task UploadAsync_StoresFileAndExtractsText()
        {
            _repo.Setup(r => r.AddAsync(It.IsAny<KnowledgeDocument>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((KnowledgeDocument d, CancellationToken _) => d);

            using var stream = new MemoryStream("Hello world"u8.ToArray());

            var result = await _sut.UploadAsync(
                "Test Doc", "Description", "test.txt", "text/plain", stream, stream.Length);

            result.Title.Should().Be("Test Doc");
            _storage.Verify(s => s.EnsureBucketExistsAsync("clinic-knowledge", It.IsAny<CancellationToken>()), Times.Once);
            _storage.Verify(s => s.UploadAsync(
                "clinic-knowledge", It.IsAny<string>(), It.IsAny<Stream>(), "text/plain",
                It.IsAny<CancellationToken>()), Times.Once);
            _repo.Verify(r => r.AddAsync(
                It.Is<KnowledgeDocument>(d => d.Title == "Test Doc"),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UploadAsync_UnsupportedType_NoTextExtracted()
        {
            _repo.Setup(r => r.AddAsync(It.IsAny<KnowledgeDocument>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((KnowledgeDocument d, CancellationToken _) => d);

            using var stream = new MemoryStream(new byte[] { 0x00 });

            var result = await _sut.UploadAsync(
                "Image", null, "photo.jpg", "image/jpeg", stream, 1);

            _textExtractor.Verify(e => e.ExtractTextAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_NotFound_ThrowsKeyNotFoundException()
        {
            _repo.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
                .ReturnsAsync((KnowledgeDocument?)null);

            var act = () => _sut.UpdateAsync(99, new UpdateKnowledgeDocumentDto("Title", null, true));

            await act.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task UpdateAsync_UpdatesDocument()
        {
            var doc = KnowledgeDocument.Create("Old", "file.txt", "text/plain", "bkey1", "key", 100);
            _repo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(doc);
            _repo.Setup(r => r.UpdateAsync(It.IsAny<KnowledgeDocument>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((KnowledgeDocument d, CancellationToken _) => d);

            var result = await _sut.UpdateAsync(1, new UpdateKnowledgeDocumentDto("New Title", "Desc", false));

            result.Title.Should().Be("New Title");
            result.IsActive.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteAsync_NotFound_ThrowsKeyNotFoundException()
        {
            _repo.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
                .ReturnsAsync((KnowledgeDocument?)null);

            var act = () => _sut.DeleteAsync(99);

            await act.Should().ThrowAsync<KeyNotFoundException>();
        }
    }
}

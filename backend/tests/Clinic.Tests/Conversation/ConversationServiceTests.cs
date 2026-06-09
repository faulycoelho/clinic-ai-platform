using Clinic.Application.DTOs;
using Clinic.Application.Interfaces;
using Clinic.Application.Services;
using FluentAssertions;
using Moq;

namespace Clinic.Tests.Conversation
{
    public class ConversationServiceTests
    {
        private readonly Mock<IConversationRepository> _repoMock = new();
        private readonly ConversationService _sut;

        public ConversationServiceTests()
        {
            _sut = new ConversationService(_repoMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsMappedDtos()
        {
            var conversations = new List<Domain.Conversation>
        {
            Domain.Conversation.Create("John", "5561999999999"),
            Domain.Conversation.Create("Jane", "5561999999998")
        };
            _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(conversations);

            var result = await _sut.GetAllAsync();

            result.Should().HaveCount(2);
            result[0].ContactName.Should().Be("John");
            result[1].ContactName.Should().Be("Jane");
        }

        [Fact]
        public async Task GetByIdAsync_Found_ReturnsDetailDto()
        {
            var conv = Domain.Conversation.Create("John", "5561999999999");
            _repoMock.Setup(r => r.GetByIdWithMessagesAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(conv);

            var result = await _sut.GetByIdAsync(1);

            result.ContactName.Should().Be("John");
            result.ContactPhone.Should().Be("5561999999999");
            result.Messages.Should().BeEmpty();
        }

        [Fact]
        public async Task GetByIdAsync_NotFound_Throws()
        {
            _repoMock.Setup(r => r.GetByIdWithMessagesAsync(99, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Conversation?)null);

            var act = () => _sut.GetByIdAsync(99);

            await act.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task CreateAsync_Valid_ReturnsDto()
        { 
            _repoMock.Setup(r => r.AddAsync(It.IsAny<Domain.Conversation>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Conversation c, CancellationToken _) => c);

            var result = await _sut.CreateAsync(new CreateConversationDto("John", "5561999999999"));
            
            result.ContactName.Should().Be("John");
            result.Status.Should().Be( Domain.Enums.ConversationEnums.ConversationStatus.Active);
        }
         
        [Fact]
        public async Task SendMessageAsync_Valid_ReturnsDetailWithMessage()
        {
            var conv = Domain.Conversation.Create("ext-1", "5561999999999");
            _repoMock.Setup(r => r.GetByIdWithMessagesAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(conv);
            _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Domain.Conversation>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Conversation c, CancellationToken _) => c);

            var result = await _sut.SendMessageAsync(1, new SendMessageDto("Hello"));

            result.Messages.Should().HaveCount(1);
            result.Messages[0].Content.Should().Be("Hello");
        }

        [Fact]
        public async Task SendMessageAsync_NotFound_Throws()
        {
            _repoMock.Setup(r => r.GetByIdWithMessagesAsync(99, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Conversation?)null);

            var act = () => _sut.SendMessageAsync(99, new SendMessageDto("Hello"));

            await act.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task CloseAsync_Valid_ReturnsClosed()
        {
            var conv = Domain.Conversation.Create("John", "5561999999999");
            _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(conv);
            _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Domain.Conversation>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Conversation c, CancellationToken _) => c);

            var result = await _sut.CloseAsync(1);

            result.Status.Should().Be(Domain.Enums.ConversationEnums.ConversationStatus.Closed);
        }

        [Fact]
        public async Task CloseAsync_NotFound_Throws()
        {
            _repoMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Conversation?)null);

            var act = () => _sut.CloseAsync(99);

            await act.Should().ThrowAsync<KeyNotFoundException>();
        }
    }
}

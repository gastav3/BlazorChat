using BlazorChatAPI.Repositories;
using BlazorChatAPI.Services;
using BlazorChatShared.Models.Entities;
using Moq;

namespace BlazorChat.Tests.Services;

public class ChatServiceTests
{
    private readonly Mock<IChatRepository> _chatRepositoryMock;
    private readonly ChatService _chatService;

    public ChatServiceTests()
    {
        _chatRepositoryMock = new Mock<IChatRepository>();
        _chatService = new ChatService(_chatRepositoryMock.Object);
    }

    [Fact]
    public async Task AddMessage_ShouldSetTimestampAndCallRepository()
    {
        // Arrange
        var msg = new ChatMessageEntity { User = "Sven", Message = "Hello" };
        _chatRepositoryMock.Setup(repo => repo.AddMessage(It.IsAny<ChatMessageEntity>()))
            .ReturnsAsync((ChatMessageEntity m) => m);

        // Act
        var result = await _chatService.AddMessage(msg);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(default, result.Timestamp);
        Assert.Equal("Sven", result.User);
        Assert.Equal("Hello", result.Message);
        _chatRepositoryMock.Verify(repo => repo.AddMessage(It.IsAny<ChatMessageEntity>()), Times.Once);
    }

    [Fact]
    public async Task AddMessage_ShouldThrowArgumentNullException_WhenMsgIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _chatService.AddMessage(null!));
    }

    [Fact]
    public async Task GetMessagesById_ShouldReturnMessage_WhenIdIsValidGuid()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        var msg = new ChatMessageEntity { Message = "Test" };
        _chatRepositoryMock.Setup(repo => repo.GetMessagesById(It.IsAny<Guid>()))
            .ReturnsAsync(msg);

        // Act
        var result = await _chatService.GetMessagesById(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test", result.Message);
        _chatRepositoryMock.Verify(repo => repo.GetMessagesById(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task GetMessagesById_ShouldReturnNull_WhenIdIsInvalid()
    {
        var result = await _chatService.GetMessagesById("invalid-guid");
        Assert.Null(result);
        _chatRepositoryMock.Verify(repo => repo.GetMessagesById(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task GetMessagesByRoomId_ShouldReturnMessages_WhenIdIsValidGuid()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        var messages = new List<ChatMessageEntity>
        {
            new ChatMessageEntity { Message = "Msg1" },
            new ChatMessageEntity { Message = "Msg2" }
        };
        _chatRepositoryMock.Setup(repo => repo.GetMessagesByRoomId(It.IsAny<Guid>(), 1, 10))
            .ReturnsAsync((messages, false));

        // Act
        var (resultMessages, hasMore) = await _chatService.GetMessagesByRoomId(id, 1, 10);

        // Assert
        Assert.NotEmpty(resultMessages);
        Assert.False(hasMore);
        _chatRepositoryMock.Verify(repo => repo.GetMessagesByRoomId(It.IsAny<Guid>(), 1, 10), Times.Once);
    }

    [Fact]
    public async Task GetMessagesByRoomId_ShouldReturnEmpty_WhenIdIsInvalid()
    {
        var (messages, hasMore) = await _chatService.GetMessagesByRoomId("invalid", 1, 10);
        Assert.Empty(messages);
        Assert.False(hasMore);
        _chatRepositoryMock.Verify(repo => repo.GetMessagesByRoomId(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }
}

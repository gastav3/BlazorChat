
using BlazorChatAPI.Hubs;
using BlazorChatAPI.Repositories;
using BlazorChatAPI.Services;
using BlazorChatAPI.State;
using BlazorChatShared.Models.Entities;
using Moq;

namespace BlazorChat.Tests.Services;

public class RoomServiceTests
{
    private readonly Mock<IRoomRepository> _roomRepositoryMock;
    private readonly Mock<IHubApi> _hubApiMock;
    private readonly Mock<IRoomState> _roomStateMock;
    private readonly RoomService _roomService;

    public RoomServiceTests()
    {
        _roomRepositoryMock = new Mock<IRoomRepository>();
        _hubApiMock = new Mock<IHubApi>();
        _roomStateMock = new Mock<IRoomState>();
        _roomService = new RoomService(_roomRepositoryMock.Object, _hubApiMock.Object, _roomStateMock.Object);
    }

    [Fact]
    public async Task CreateRoom_ShouldThrowArgumentNullException_WhenRoomIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _roomService.CreateRoom(null!));
    }

    [Fact]
    public async Task CreateRoom_ShouldCallRepositoryAndHubApi()
    {
        // Arrange
        var room = new RoomEntity { Id = Guid.NewGuid(), Name = "TestRoom" };
        _roomRepositoryMock.Setup(r => r.CreateRoom(It.IsAny<RoomEntity>())).ReturnsAsync(room);
        _hubApiMock.Setup(h => h.RegisterOnRoomUpdate(It.IsAny<RoomEntity>())).Returns(Task.CompletedTask);

        // Act
        var result = await _roomService.CreateRoom(room);

        // Assert
        Assert.Equal(room, result);
        _roomRepositoryMock.Verify(r => r.CreateRoom(room), Times.Once);
        _hubApiMock.Verify(h => h.RegisterOnRoomUpdate(room), Times.Once);
    }

    [Fact]
    public async Task GetAllRooms_ShouldReturnRoomsWithConnections()
    {
        // Arrange
        var rooms = new List<RoomEntity>
        {
            new RoomEntity { Id = Guid.NewGuid(), Name = "Room1" },
            new RoomEntity { Id = Guid.NewGuid(), Name = "Room2" }
        };

        _roomRepositoryMock.Setup(r => r.GetAllRooms(It.IsAny<bool>())).ReturnsAsync(rooms);

        _roomStateMock.Setup(s => s.GetConnectionsInRoom(It.IsAny<string>()))
            .Returns((string roomId) => new List<string> { $"Conn-{roomId}-1", $"Conn-{roomId}-2" });

        // Act
        var result = (await _roomService.GetAllRooms()).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        foreach (var room in result)
        {
            Assert.NotNull(room.Connections);
            Assert.Equal(2, room.Connections.Count);
            Assert.All(room.Connections, c => Assert.StartsWith("Conn-", c));
        }
    }

    [Fact]
    public async Task GetRoomById_ShouldReturnRoomWithConnections_WhenIdIsValid()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var idString = guid.ToString();
        var room = new RoomEntity { Id = guid, Name = "Room1" };
        var connections = new List<string> { "conn1", "conn2" };

        _roomRepositoryMock.Setup(r => r.GetRoomById(guid, false)).ReturnsAsync(room);
        _roomStateMock.Setup(s => s.GetConnectionsInRoom(idString)).Returns(connections);

        // Act
        var result = await _roomService.GetRoomById(idString);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(room.Id, result.Id);
        Assert.Equal(connections, result.Connections);
    }

    [Fact]
    public async Task GetRoomById_ShouldReturnNull_WhenIdIsInvalid()
    {
        var result = await _roomService.GetRoomById("invalid-guid");
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateRoom_ShouldThrowArgumentNullException_WhenRoomIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _roomService.UpdateRoom(null!));
    }

    [Fact]
    public async Task UpdateRoom_ShouldCallRepository()
    {
        // Arrange
        var room = new RoomEntity { Id = Guid.NewGuid(), Name = "RoomUpdate" };
        _roomRepositoryMock.Setup(r => r.UpdateRoom(room)).ReturnsAsync(room);

        // Act
        var result = await _roomService.UpdateRoom(room);

        // Assert
        Assert.Equal(room, result);
        _roomRepositoryMock.Verify(r => r.UpdateRoom(room), Times.Once);
    }
}

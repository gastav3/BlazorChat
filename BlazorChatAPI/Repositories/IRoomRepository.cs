using BlazorChatShared.Models.Entities;

namespace BlazorChatAPI.Repositories;

public interface IRoomRepository
{
    Task<List<RoomEntity>> GetAllRooms();
    Task<RoomEntity?> GetRoomById(Guid id);
    Task<RoomEntity> CreateRoom(RoomEntity lobby);
}

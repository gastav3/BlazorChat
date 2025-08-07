using BlazorChatShared.Models.Entities;
using BlazorChatShared.Parameters;

namespace BlazorChatAPI.Services;

public interface IRoomService
{
    Task<RoomEntity> CreateRoom(RoomEntity room);
    Task<IEnumerable<RoomEntity>> GetAllRooms();
    Task<RoomEntity?> GetRoomById(string id);
}

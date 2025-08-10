using BlazorChatShared.Models.Entities;

namespace BlazorChatAPI.Services;

public interface IRoomService
{
    Task<RoomEntity> CreateRoom(RoomEntity room);
    Task<IEnumerable<RoomEntity>> GetAllRooms(bool showHidden = false);
    Task<RoomEntity?> UpdateRoom(RoomEntity room);
    Task<RoomEntity?> GetRoomById(string id, bool showHidden = false);
}

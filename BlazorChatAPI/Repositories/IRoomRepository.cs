using BlazorChatShared.Models.Entities;

namespace BlazorChatAPI.Repositories;

public interface IRoomRepository
{
    Task<List<RoomEntity>> GetAllRooms(bool showHidden = false);
    Task<RoomEntity?> GetRoomById(Guid id, bool showHidden = false);
    Task<RoomEntity?> UpdateRoom(RoomEntity updatedRoom);
    Task<RoomEntity> CreateRoom(RoomEntity room);
}

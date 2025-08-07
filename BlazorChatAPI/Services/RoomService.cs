using AutoMapper;
using BlazorChatAPI.Repositories;
using BlazorChatShared.Models.Entities;
using BlazorChatShared.Models.Models;

namespace BlazorChatAPI.Services;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;

    public RoomService(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository ?? throw new ArgumentNullException(nameof(roomRepository));

    }
    public async Task<RoomEntity> CreateRoom(RoomEntity lobby)
    {
        if(lobby == null)
        {
            throw new ArgumentNullException(nameof(lobby), "Room cannot be null");
        }

        return await _roomRepository.CreateRoom(lobby);
    }

    public async Task<IEnumerable<RoomEntity>> GetAllRooms()
    {
            return await _roomRepository.GetAllRooms();
    }

    public async Task<RoomEntity?> GetRoomById(string id)
    {
        if (!string.IsNullOrEmpty(id) && Guid.TryParse(id, out Guid guid))
        {
            return await _roomRepository.GetRoomById(guid);
        }
        return null;
    }
}

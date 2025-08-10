using BlazorChatAPI.Hubs;
using BlazorChatAPI.Repositories;
using BlazorChatAPI.State;
using BlazorChatShared.Models.Entities;

namespace BlazorChatAPI.Services;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IHubApi _hubApi;
    private readonly IRoomState _roomState;

    public RoomService(IRoomRepository roomRepository, IHubApi hubApi, IRoomState roomState)
    {
        _roomRepository = roomRepository ?? throw new ArgumentNullException(nameof(roomRepository));
        _hubApi = hubApi ?? throw new ArgumentNullException(nameof(hubApi));
        _roomState = roomState ?? throw new ArgumentNullException(nameof(roomState));

    }
    public async Task<RoomEntity> CreateRoom(RoomEntity room)
    {
        if (room == null)
        {
            throw new ArgumentNullException(nameof(room), "Room cannot be null");
        }

        var createdRoom = await _roomRepository.CreateRoom(room);
        await _hubApi.RegisterOnRoomUpdate(createdRoom);

        return createdRoom;
    }

    public async Task<IEnumerable<RoomEntity>> GetAllRooms(bool showHidden = false)
    {
        var rooms = await _roomRepository.GetAllRooms(showHidden);

        foreach(var room in rooms)
        {
            room.Connections = _roomState.GetConnectionsInRoom(room.Id.ToString()).ToList();
        }

        return rooms;
    }

    public async Task<RoomEntity?> GetRoomById(string id, bool showHidden = false)
    {
        if (!string.IsNullOrEmpty(id) && Guid.TryParse(id, out Guid guid))
        {
            var room = await _roomRepository.GetRoomById(guid, showHidden);

            if(room != null)
            {
                room.Connections = [.. _roomState.GetConnectionsInRoom(id)];
            }
            return room;
        }
        return null;
    }

    public async Task<RoomEntity?> UpdateRoom(RoomEntity room)
    {
        if (room == null)
        {
            throw new ArgumentNullException(nameof(room), "Room cannot be null");
        }

        return await _roomRepository.UpdateRoom(room);
    }
}

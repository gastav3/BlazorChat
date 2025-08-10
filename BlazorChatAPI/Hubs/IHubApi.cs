using BlazorChatShared.Models.Entities;
using BlazorChatShared.Models.Models;

namespace BlazorChatAPI.Hubs;

public interface IHubApi
{
    Task RegisterOnRoomUpdate(RoomEntity room);
}

using BlazorChatShared.Models.Models;
using BlazorChatShared.Parameters;

namespace BlazorChatWeb.WebServices;

public interface IChatRoomWebService
{
    Task<List<Room>> GetAllRooms();
    Task<Room?> GetRoomById(string id);
    Task<Room?> CreateRoom(CreateRoomParameter request);
}

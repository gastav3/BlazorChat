using BlazorChatShared.Models.Models;
using BlazorChatShared.Parameters;

namespace BlazorChatWeb.WebServices;

public interface IChatRoomWebService
{
    Task<List<Room>> GetAllRoomsAsync();
    Task<Room?> GetRoomByIdAsync(string id);
    Task<Room?> CreateRoomAsync(CreateRoomParameter request);
}

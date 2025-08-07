using BlazorChatShared.Models.Models;

namespace BlazorChatWeb.WebServices;

public interface IMessageWebService
{
    Task<List<ChatMessage>> GetChatMessagesByRoomId(string id);
}

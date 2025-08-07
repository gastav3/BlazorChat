using BlazorChatShared.Models.Models;
using BlazorChatShared.Parameters;

namespace BlazorChatAPI.Hubs;

public interface IChatHub
{
    Task ReceiveMessage(ChatMessage msg);
    Task ReceiveMessagesPageing(PagedMessagesResultParameter messages);
}

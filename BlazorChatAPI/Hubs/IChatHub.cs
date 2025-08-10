using BlazorChatShared.Models.Models;
using BlazorChatShared.Parameters;
using System.Collections.Generic;

namespace BlazorChatAPI.Hubs;

public interface IChatHub
{
    Task ReceiveMessage(ChatMessage msg);
    Task ReceiveMessagesPageing(PagedMessagesResultParameter messages);
    Task ReceiveUpdatedRoom(Room room);
}

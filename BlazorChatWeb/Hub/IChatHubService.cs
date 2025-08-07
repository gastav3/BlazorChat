using BlazorChatShared.Models.Models;
using BlazorChatShared.Parameters;
using Microsoft.AspNetCore.SignalR.Client;

namespace BlazorChatWeb.Hub;

public interface IChatHubService
{
    HubConnection? Connection { get; }
    event Func<ChatMessage, Task>? OnMessageReceived;
    Task StartConnection(string hubUrl);
    Task SendMessage(ChatMessage msg);
    Task<PagedMessagesResultParameter?> LoadMessages(string roomId, int currentPage, int pageSize);
}

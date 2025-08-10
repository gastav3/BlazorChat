using BlazorChatShared.Models.Models;
using BlazorChatShared.Parameters;
using Microsoft.AspNetCore.SignalR.Client;

namespace BlazorChatWeb.Hub;

public class ChatHubService : IChatHubService, IAsyncDisposable
{
    public HubConnection? Connection { get; private set; }

    public event Func<ChatMessage, Task>? OnMessageReceived;
    public event Func<Room, Task>? OnRoomReceived;

    public async Task SendMessage(ChatMessage msg)
    {
        if (Connection != null && Connection.State == HubConnectionState.Connected)
        {
            await Connection.SendAsync("SendMessage", msg);
        }
    }
    public async Task RequestUpdate(string roomId)
    {
        if (Connection != null && Connection.State == HubConnectionState.Connected)
        {
            await Connection.InvokeAsync("UpdateRoom", roomId);
        }
    }

    public async Task<PagedMessagesResultParameter?> LoadMessages(string roomId, int currentPage, int pageSize)
    {
        if (Connection != null && Connection.State == HubConnectionState.Connected)
        {
            return await Connection.InvokeAsync<PagedMessagesResultParameter>("LoadMessages", roomId, currentPage, pageSize);
        }

        return null;
    }

    public async Task StartConnection(string hubUrl)
    {
        if (Connection == null || Connection.State == HubConnectionState.Disconnected)
        {
            Connection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .WithAutomaticReconnect()
                .Build();

            RegisterHubHandlers();

            await Connection.StartAsync();
        }
    }

    private void RegisterHubHandlers()
    {
        Connection?.On<ChatMessage>("ReceiveMessage", async (msg) =>
        {
            if (OnMessageReceived != null)
            {
                await OnMessageReceived.Invoke(msg);
            }
        });

        Connection?.On<Room>("ReceiveUpdatedRoom", async (room) =>
        {
            if (OnRoomReceived != null)
            {
                await OnRoomReceived.Invoke(room);
            }
        });
    }
    public async ValueTask DisposeAsync()
    {
        if (Connection != null)
        {
            await Connection.DisposeAsync();
            Connection = null;
        }
    }
}

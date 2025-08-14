using BlazorChatShared.Constants;
using BlazorChatShared.Models.Models;
using BlazorChatWeb.Hub;
using BlazorChatWeb.StateServices;
using BlazorChatWeb.WebServices;
using Microsoft.AspNetCore.Components;

namespace BlazorChatWeb.Pages;

public partial class Index : ComponentBase, IDisposable
{
    [CascadingParameter]
    public Room? SelectedRoom { get; set; } = default!;

    [Inject]
    private AppSettings AppSettings { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    [Inject]
    private IChatRoomWebService ChatRoomWebService { get; set; } = default!;

    [Inject]
    private IChatHubService ChatHubService { get; set; } = default!;

    private Func<Room, Task>? _roomReceivedHandler;
    private List<Room> rooms = [];

    protected override async Task OnParametersSetAsync()
    {
        await ChatHubService.RequestUpdate(ChatConstants.MainRoomId.ToString());
        await base.OnParametersSetAsync();
    }

    protected override async Task OnInitializedAsync()
    {
        rooms = await ChatRoomWebService.GetAllRooms();
        await ChatHubService.StartConnection(AppSettings.HubUrl);

        _roomReceivedHandler = async (updatedRoom) =>
        {
            var existingRoom = rooms.FirstOrDefault(x => x.Id == updatedRoom.Id);
            if (existingRoom != null)
            {
                existingRoom.Connections = updatedRoom.Connections;
                existingRoom.Description = updatedRoom.Description;

                await InvokeAsync(StateHasChanged);
                return;
            }

            rooms.Add(updatedRoom);
            await InvokeAsync(StateHasChanged);
        };

        ChatHubService.OnRoomReceived += _roomReceivedHandler;
    }

    private int GetTotalConnectionCount()
    {
        var indexRoom = rooms.FirstOrDefault(x => x.Id == ChatConstants.MainRoomId.ToString());
        return indexRoom?.Connections.Count ?? 0;
    }

    private void JoinRoom(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Room ID cannot be null or empty.", nameof(id));
        }

        Navigation.NavigateTo($"/chat/{id}");
    }

    public void Dispose()
    {
        if (_roomReceivedHandler != null)
        {
            ChatHubService.OnRoomReceived -= _roomReceivedHandler;
        }
    }
}

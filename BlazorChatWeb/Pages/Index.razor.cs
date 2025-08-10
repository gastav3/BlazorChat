using AutoMapper;
using BlazorChatShared.Constants;
using BlazorChatShared.Models.Models;
using BlazorChatWeb.Hub;
using BlazorChatWeb.WebServices;
using Microsoft.AspNetCore.Components;

namespace BlazorChatWeb.Pages;

public partial class Index : ComponentBase, IDisposable
{
    [CascadingParameter]
    public Room? SelectedRoom { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    [Inject]
    private IMapper _mapper { get; set; } = default!;

    [Inject]
    private IChatRoomWebService _chatRoomWebService { get; set; } = default!;

    [Inject]
    private IChatHubService chatHubService { get; set; } = default!;
    private Func<Room, Task>? _roomReceivedHandler;
    private List<Room> rooms = [];

    protected override async Task OnParametersSetAsync()
    {
        await chatHubService.RequestUpdate(ChatConstants.MainRoomId.ToString());
        await base.OnParametersSetAsync();
    }

    protected override async Task OnInitializedAsync()
    {
        rooms = await _chatRoomWebService.GetAllRooms();
        await chatHubService.StartConnection("https://localhost:7199/chathub");

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

        chatHubService.OnRoomReceived += _roomReceivedHandler;
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
            chatHubService.OnRoomReceived -= _roomReceivedHandler;
        }
    }
}

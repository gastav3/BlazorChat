using BlazorChatShared.Models.Models;
using BlazorChatShared.Parameters;
using BlazorChatWeb.Hub;
using BlazorChatWeb.StateServices;
using BlazorChatWeb.WebServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace BlazorChatWeb.Pages;

public partial class Index : ComponentBase
{
    [CascadingParameter]
    public Room? SelectedRoom { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    [Inject]
    private IRoomState _roomState { get; set; } = default!;

    [Inject]
    private IChatRoomWebService _chatRoomWebService { get; set; } = default!;

    private List<Room> rooms = [];

    private string newRoomName = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        rooms = await _chatRoomWebService.GetAllRoomsAsync();
    }

    private async Task JoinRoom(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Room ID cannot be null or empty.", nameof(id));
        }
        _roomState.SelectedRoom = await _chatRoomWebService.GetRoomByIdAsync(id);
        Navigation.NavigateTo($"/chat/{id}");
    }

    private async Task CreateRoom()
    {
        var newRoomParameter = new CreateRoomParameter
        {
            Name = newRoomName
        };

        await _chatRoomWebService.CreateRoomAsync(newRoomParameter);
    }
}

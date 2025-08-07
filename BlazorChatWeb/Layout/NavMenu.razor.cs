using BlazorChatShared.Models.Models;
using BlazorChatShared.Parameters;
using BlazorChatWeb.Hub;
using BlazorChatWeb.StateServices;
using BlazorChatWeb.WebServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace BlazorChatWeb.Layout;

public partial class NavMenu : ComponentBase
{
    [CascadingParameter]
    public Room? SelectedRoom { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    [Inject]
    private IChatRoomWebService _chatRoomWebService { get; set; } = default!;

    [Inject]
    private IRoomState _roomState { get; set; } = default!;

    [Inject]
    private IChatHubService chatHubService { get; set; } = default!;
    private List<Room> rooms = [];

    private string newRoomName = string.Empty;
    private bool collapseNavMenu = true;
    private string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;

    protected override async Task OnInitializedAsync()
    {
        rooms = await _chatRoomWebService.GetAllRoomsAsync();
        await chatHubService.StartConnection("https://localhost:7199/chathub");
    }

    private async Task JoinRoom(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Room ID cannot be null or empty.", nameof(id));
        }

        if (chatHubService.Connection == null)
        {
            throw new InvalidOperationException("SignalR connection has not been initialized.");
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

    private void ToggleNavMenu()
    {
        collapseNavMenu = !collapseNavMenu;
    }
}

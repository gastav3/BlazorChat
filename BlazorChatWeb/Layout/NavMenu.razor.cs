using BlazorChatShared.Models.Models;
using BlazorChatShared.Parameters;
using BlazorChatWeb.Components;
using BlazorChatWeb.Hub;
using BlazorChatWeb.StateServices;
using BlazorChatWeb.WebServices;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace BlazorChatWeb.Layout;

public partial class NavMenu : ComponentBase, IDisposable
{
    [CascadingParameter]
    public Room? SelectedRoom { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    [Inject]
    private IChatRoomWebService ChatRoomWebService { get; set; } = default!;

    [Inject]
    private IRoomState RoomState { get; set; } = default!;

    [Inject]
    private AppSettings AppSettings { get; set; } = default!;

    [Inject]
    private IChatHubService ChatHubService { get; set; } = default!;

    [Inject]
    private ILocalStorageService LocalStorage { get; set; } = default!;

    private const string VisitedRoomsKey = "visitedRooms";
    private List<Room> visitedRooms = [];
    private Action? roomStateChangedHandler;

    private bool collapseNavMenu = true;
    private string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;
    private CreateRoomModalComponent? Modal;

    public async Task ShowModal()
    {
        if (Modal != null)
        {
            await Modal.ShowAsync();
        }
    }

    private async Task OnRoomCreateModalSubmit(Room newRoom)
    {
        if (newRoom == null || string.IsNullOrWhiteSpace(newRoom.Name))
        {
            return;
        }

        var newRoomParameter = new CreateRoomParameter
        {
            Name = newRoom.Name,
            Description = newRoom.Description
        };

        var createdRoom = await ChatRoomWebService.CreateRoom(newRoomParameter);
        if (createdRoom != null)
        {
            JoinRoom(createdRoom.Id);
        }
    }


    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        var uri = new Uri(e.Location);
        if (!uri.AbsolutePath.StartsWith("/chat"))
        {
            RoomState.SelectedRoom = null;
        }
        _ = LoadVisitedRoomsAsync();
    }

    private async Task LoadVisitedRoomsAsync()
    {
        var allRooms = await ChatRoomWebService.GetAllRooms();

        var storedRooms = await LocalStorage.GetItemAsync<List<Room>>(VisitedRoomsKey) ?? new List<Room>();

        visitedRooms = storedRooms
            .Where(sr => allRooms.Any(ar => ar.Id == sr.Id))
            .ToList();

        await InvokeAsync(StateHasChanged);
    }

    protected override async Task OnInitializedAsync()
    {
        await ChatHubService.StartConnection(AppSettings.HubUrl);
        await LoadVisitedRoomsAsync();

        roomStateChangedHandler = async () =>
        {
            SelectedRoom = RoomState.SelectedRoom;
            await LoadVisitedRoomsAsync();
            await InvokeAsync(StateHasChanged);
        };

        RoomState.OnChange += roomStateChangedHandler;
        Navigation.LocationChanged += OnLocationChanged;
    }

    private void JoinRoom(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Room ID cannot be null or empty.", nameof(id));
        }

        if (ChatHubService.Connection == null)
        {
            throw new InvalidOperationException("SignalR connection has not been initialized.");
        }

        Navigation.NavigateTo($"/chat/{id}");
    }

    private void ToggleNavMenu()
    {
        collapseNavMenu = !collapseNavMenu;
    }

    public void Dispose()
    {
        if (roomStateChangedHandler != null)
        {
            RoomState.OnChange -= roomStateChangedHandler;
        }
        Navigation.LocationChanged -= OnLocationChanged;
    }
}

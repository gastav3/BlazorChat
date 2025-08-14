using BlazorChatShared.Models.Models;
using BlazorChatWeb.StateServices;
using Microsoft.AspNetCore.Components;

namespace BlazorChatWeb.Layout;

public partial class MainLayout : LayoutComponentBase
{
    [Inject]
    public IRoomState RoomState { get; set; } = default!;

    public Room? SelectedRoom => RoomState.SelectedRoom;
}

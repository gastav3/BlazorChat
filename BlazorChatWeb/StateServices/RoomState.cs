using BlazorChatShared.Models.Models;

namespace BlazorChatWeb.StateServices;

public class RoomState : IRoomState
{
    public Room? SelectedRoom { get; set; }
}

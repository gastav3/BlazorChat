using BlazorChatShared.Models.Models;

namespace BlazorChatWeb.StateServices;

public interface IRoomState
{
    Room? SelectedRoom { get; set; }
    event Action? OnChange;
    void NotifyStateChanged();
}

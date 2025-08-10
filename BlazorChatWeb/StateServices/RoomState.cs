using BlazorChatShared.Models.Models;

namespace BlazorChatWeb.StateServices;

public class RoomState : IRoomState
{
    private Room? _selectedRoom;
    public Room? SelectedRoom
    {
        get => _selectedRoom;
        set
        {
            _selectedRoom = value;
            NotifyStateChanged();
        }
    }

    public event Action? OnChange;

    public void NotifyStateChanged() => OnChange?.Invoke();
}


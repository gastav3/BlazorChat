namespace BlazorChatAPI.State;

public interface IRoomState
{
    void AddConnection(string connectionId, string roomId);
    void RemoveConnection(string connectionId, string roomId);
    IEnumerable<string> GetRooms(string connectionId);
    int GetConnectionCount();
    IEnumerable<string> GetConnectionsInRoom(string roomId);
}
using System.Collections.Concurrent;

namespace BlazorChatAPI.State;

public class RoomState : IRoomState
{
    private readonly ConcurrentDictionary<string, HashSet<string>> _connections = new();

    public void AddConnection(string connectionId, string roomId)
    {
        _connections.AddOrUpdate(
            connectionId,
            _ => new HashSet<string> { roomId },
            (_, rooms) =>
            {
                lock (rooms)
                {
                    rooms.Add(roomId);
                }
                return rooms;
            });
    }

    public void RemoveConnection(string connectionId, string roomId)
    {
        if (_connections.TryGetValue(connectionId, out var rooms))
        {
            lock (rooms)
            {
                rooms.Remove(roomId);
                if (rooms.Count == 0)
                {
                    _connections.TryRemove(connectionId, out _);
                }
            }
        }
    }

    public IEnumerable<string> GetRooms(string connectionId)
    {
        if (_connections.TryGetValue(connectionId, out var rooms))
        {
            lock (rooms)
            {
                return rooms.ToList();
            }
        }
        return Enumerable.Empty<string>();
    }

    public int GetConnectionCount() => _connections.Count;

    public IEnumerable<string> GetConnectionsInRoom(string roomId)
    {
        return _connections.Where(kv => kv.Value.Contains(roomId)).Select(kv => kv.Key);
    }
}

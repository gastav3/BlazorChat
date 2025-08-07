using AutoMapper;
using BlazorChatAPI.Repositories;
using BlazorChatAPI.Services;
using BlazorChatShared.Models.Entities;
using BlazorChatShared.Models.Models;
using BlazorChatShared.Parameters;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace BlazorChatAPI.Hubs;

public class ChatHub : Hub<IChatHub>
{
    private readonly IMapper _autoMapper;
    private readonly IChatService _chatService;
    private readonly IRoomService _roomService;

    // user - rooms (thread-safe)
    private static readonly ConcurrentDictionary<string, HashSet<string>> _connectionRooms = new();
    private static readonly string[] defaultRooms = { "main_room, notification_room" };

    public ChatHub(IMapper autoMapper, IChatService chatService, IRoomService roomService)
    {
        _autoMapper = autoMapper ?? throw new ArgumentNullException(nameof(autoMapper));
        _chatService = chatService ?? throw new ArgumentNullException(nameof(chatService));
        _roomService = roomService ?? throw new ArgumentNullException(nameof(roomService));
    }

    public override async Task OnConnectedAsync()
    {
        // Optional: Log connection
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await LeaveAllRooms();
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinRoom(string roomId)
    {
        var room = await _roomService.GetRoomById(roomId);
        if (room == null)
        {
            throw new ArgumentException($"Room with ID {roomId} does not exist.", nameof(roomId));
        }

       await LeaveAllRooms(defaultRooms);

        // Add to room list
        _connectionRooms.AddOrUpdate(
            Context.ConnectionId,
            _ => new HashSet<string> { roomId },
            (_, rooms) =>
            {
                lock (rooms)
                {
                    rooms.Add(roomId);
                }
                return rooms;
            });

        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
    }

    public async Task LeaveRoom(string roomId)
    {
        var room = await _roomService.GetRoomById(roomId);
        if (room == null)
        {
            throw new ArgumentException($"Room with ID {roomId} does not exist.", nameof(roomId));
        }

        if (_connectionRooms.TryGetValue(Context.ConnectionId, out var rooms))
        {
            lock (rooms)
            {
                rooms.Remove(roomId);
            }
        }

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
    }

    public async Task<PagedMessagesResultParameter> LoadMessages(string roomId, int pageNumber, int pageSize)
    {
        var (messageEntities, hasMore) = await _chatService.GetMessagesByRoomId(roomId, pageNumber, pageSize);
        var messages = _autoMapper.Map<IEnumerable<ChatMessage>>(messageEntities);

        return new PagedMessagesResultParameter
        {
            Messages = messages,
            HasMore = hasMore
        };
    }

    public async Task SendMessage(ChatMessage msg)
    {
        if (msg == null)
        {
            throw new ArgumentNullException(nameof(msg));
        }

        if (string.IsNullOrWhiteSpace(msg.Message))
        {
            throw new ArgumentException("Message is invalid or empty.", nameof(msg.Message));
        }

        var msgEntity = _autoMapper.Map<ChatMessageEntity>(msg);
        var newMsg = await _chatService.AddMessage(msgEntity);

        // Timestamp is set by server-side service
        if (newMsg != null && newMsg.Timestamp != default)
        {
            msg.Timestamp = newMsg.Timestamp;
            await Clients.Group(msg.GroupId).ReceiveMessage(msg);
        }
    }

    private async Task LeaveAllRooms(IEnumerable<string>? excludeRooms = null)
    {
        excludeRooms ??= [];

        if (_connectionRooms.TryGetValue(Context.ConnectionId, out var rooms))
        {
            List<string> roomsToLeave;

            lock (rooms)
            {
                roomsToLeave = rooms
                    .Where(room => !excludeRooms.Contains(room))
                    .ToList();

                foreach (var room in roomsToLeave)
                {
                    rooms.Remove(room);
                }

                // Only remove the whole entry if no rooms left
                if (rooms.Count == 0)
                {
                    _connectionRooms.TryRemove(Context.ConnectionId, out _);
                }
            }

            foreach (var room in roomsToLeave)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, room);
            }
        }
    }

}

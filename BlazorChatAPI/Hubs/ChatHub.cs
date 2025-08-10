using AutoMapper;
using BlazorChatAPI.Repositories;
using BlazorChatAPI.Services;
using BlazorChatAPI.State;
using BlazorChatShared.Constants;
using BlazorChatShared.Models.Entities;
using BlazorChatShared.Models.Models;
using BlazorChatShared.Parameters;
using Microsoft.AspNetCore.SignalR;

namespace BlazorChatAPI.Hubs;

public class ChatHub : Hub<IChatHub>
{
    private readonly IMapper _autoMapper;
    private readonly IChatService _chatService;
    private readonly IRoomService _roomService;
    private readonly IRoomState _roomState;

    public ChatHub(IMapper autoMapper, IChatService chatService, IRoomService roomService, IRoomState roomState)
    {
        _autoMapper = autoMapper ?? throw new ArgumentNullException(nameof(autoMapper));
        _chatService = chatService ?? throw new ArgumentNullException(nameof(chatService));
        _roomService = roomService ?? throw new ArgumentNullException(nameof(roomService));
        _roomState = roomState ?? throw new ArgumentNullException(nameof(roomState));
    }

    public override async Task OnConnectedAsync()
    {
        _roomState.AddConnection(Context.ConnectionId, ChatConstants.MainRoomId.ToString());
        await Groups.AddToGroupAsync(Context.ConnectionId, ChatConstants.MainRoomId.ToString());
        await UpdateRoomInfo(ChatConstants.MainRoomId.ToString());
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

        await LeaveAllRooms([ChatConstants.MainRoomId.ToString()]);

        _roomState.AddConnection(Context.ConnectionId, roomId);
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

        await UpdateRoomInfo(roomId);
    }

    public async Task LeaveRoom(string roomId)
    {
        var room = await _roomService.GetRoomById(roomId);
        if (room == null)
        {
            throw new ArgumentException($"Room with ID {roomId} does not exist.", nameof(roomId));
        }

        _roomState.RemoveConnection(Context.ConnectionId, roomId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);

        await UpdateRoomInfo(roomId);
    }

    public async Task UpdateRoom(string roomId)
    {
        var room = await _roomService.GetRoomById(roomId, true);
        if (room == null)
        {
            throw new ArgumentException($"Room with ID {roomId} does not exist.", nameof(roomId));
        }

        await UpdateRoomInfo(roomId);
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

    private async Task UpdateRoomInfo(string roomId)
    {
        var connections = _roomState.GetConnectionsInRoom(roomId);
        var roomEntity = await _roomService.GetRoomById(roomId, true) ?? throw new ArgumentException($"Room with ID {roomId} does not exist.", nameof(roomId));
        
        var room = _autoMapper.Map<Room>(roomEntity);
        room.Connections = [.. connections];

        await Clients.Group(ChatConstants.MainRoomId.ToString()).ReceiveUpdatedRoom(room);
        await Clients.Group(roomId).ReceiveUpdatedRoom(room);
    }

    private async Task LeaveAllRooms(IEnumerable<string>? excludeRooms = null)
    {
        excludeRooms ??= Array.Empty<string>();

        var roomsToLeave = _roomState.GetRooms(Context.ConnectionId)
            .Where(room => !excludeRooms.Contains(room))
            .ToList();

        foreach (var room in roomsToLeave)
        {
            _roomState.RemoveConnection(Context.ConnectionId, room);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, room);

            await UpdateRoomInfo(room);
        }
    }
}

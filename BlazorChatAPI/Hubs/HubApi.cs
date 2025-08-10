using AutoMapper;
using BlazorChatAPI.Hubs;
using BlazorChatShared.Constants;
using BlazorChatShared.Models.Entities;
using BlazorChatShared.Models.Models;
using Microsoft.AspNetCore.SignalR;

public class HubApi : IHubApi
{
    private readonly IHubContext<ChatHub, IChatHub> _hubContext;
    private readonly IMapper _autoMapper;

    public HubApi(IHubContext<ChatHub, IChatHub> hubContext, IMapper autoMapper)
    {
        _hubContext = hubContext;
        _autoMapper = autoMapper ?? throw new ArgumentNullException(nameof(autoMapper));
    }

    public async Task RegisterOnRoomUpdate(RoomEntity roomEntity)
    {
        var room = _autoMapper.Map<Room>(roomEntity);

        await _hubContext.Clients.Groups(roomEntity.Id.ToString()).ReceiveUpdatedRoom(room);
        await _hubContext.Clients.Groups(ChatConstants.MainRoomId.ToString()).ReceiveUpdatedRoom(room);
    }
}
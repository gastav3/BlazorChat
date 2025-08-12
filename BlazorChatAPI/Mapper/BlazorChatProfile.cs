using AutoMapper;
using BlazorChatShared.Models.Entities;
using BlazorChatShared.Models.Models;

namespace BlazorChatShared.Mapper;

public class BlazorChatProfile : Profile
{
    public BlazorChatProfile()
    {
        CreateMap<RoomEntity, Room>()
            .ReverseMap();

        CreateMap<ChatMessageEntity, ChatMessage>()
            .ReverseMap();
    }
}

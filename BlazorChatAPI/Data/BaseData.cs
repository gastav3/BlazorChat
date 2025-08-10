using BlazorChatShared.Constants;
using BlazorChatShared.Models.Entities;

namespace BlazorChatAPI.Data;

public static class BaseData
{
    public static void Seed(ApplicationDbContext context)
    {
        context.Database.EnsureCreated();

        if (!context.Rooms.Any(r => r.Id == ChatConstants.MainRoomId))
        {
            context.Rooms.Add(new RoomEntity
            {
                Id = ChatConstants.MainRoomId,
                Name = ChatConstants.MainRoomName,
                Description = "This is the index page room",
                Hidden = true,
                Owner = "System"
            });

            context.SaveChanges();
        }
    }
}
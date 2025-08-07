using BlazorChatShared.Models.Entities;

namespace BlazorChatAPI.Services;

public interface IChatService
{
    Task<ChatMessageEntity?> AddMessage(ChatMessageEntity msg);
    Task<ChatMessageEntity?> GetMessagesById(string id);
    Task<(IEnumerable<ChatMessageEntity> Messages, bool HasMore)> GetMessagesByRoomId(string id, int pageNumber, int pageSize);
}

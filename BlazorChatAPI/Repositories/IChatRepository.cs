using BlazorChatShared.Models.Entities;

namespace BlazorChatAPI.Repositories;

public interface IChatRepository
{
    Task<ChatMessageEntity?> GetMessagesById(Guid id);
    Task<(IEnumerable<ChatMessageEntity> Messages, bool HasMore)> GetMessagesByRoomId(Guid id, int pageNumber, int pageSize);
    Task<ChatMessageEntity?> AddMessage(ChatMessageEntity msg);
}

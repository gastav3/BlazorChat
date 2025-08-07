using BlazorChatAPI.Repositories;
using BlazorChatShared.Models.Entities;
using System;

namespace BlazorChatAPI.Services;

public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepository;

    public ChatService(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository ?? throw new ArgumentNullException(nameof(chatRepository));
    }

    public async Task<ChatMessageEntity?> AddMessage(ChatMessageEntity msg)
    {
        if (msg == null)
        {
            throw new ArgumentNullException(nameof(msg), "Room cannot be null");
        }

        msg.Timestamp = DateTime.UtcNow;

        return await _chatRepository.AddMessage(msg);
    }

    public async Task<ChatMessageEntity?> GetMessagesById(string id)
    {
        if (!string.IsNullOrEmpty(id) && Guid.TryParse(id, out Guid guid))
        {
            return await _chatRepository.GetMessagesById(guid);

        }
        return null;
    }

    public async Task<(IEnumerable<ChatMessageEntity> Messages, bool HasMore)> GetMessagesByRoomId(string id, int pageNumber, int pageSize)
    {
        if (!string.IsNullOrEmpty(id) && Guid.TryParse(id, out Guid guid))
        {
            return await _chatRepository.GetMessagesByRoomId(guid, pageNumber, pageSize);
        }

        return (Enumerable.Empty<ChatMessageEntity>(), false);
    }

}

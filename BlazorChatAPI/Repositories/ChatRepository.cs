using BlazorChatAPI.Data;
using BlazorChatShared.Models.Entities;
using BlazorChatShared.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorChatAPI.Repositories;

public class ChatRepository : IChatRepository
{
    private readonly ApplicationDbContext _context;

    public ChatRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<(IEnumerable<ChatMessageEntity> Messages, bool HasMore)> GetMessagesByRoomId(Guid id, int pageNumber, int pageSize)
    {
        var totalMessages = await _context.Messages.CountAsync(m => m.GroupId == id);

        var messages = await _context.Messages
            .Where(m => m.GroupId == id)
            .OrderByDescending(m => m.Timestamp)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        bool hasMore = (pageNumber * pageSize) < totalMessages;

        return (messages, hasMore);
    }


    public async Task<ChatMessageEntity?> AddMessage(ChatMessageEntity msg)
    {
        _context.Messages.Add(msg);
        await _context.SaveChangesAsync();
        return msg;
    }

    public async Task<ChatMessageEntity?> GetMessagesById(Guid id)
    {
        var message = await _context.Messages
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

        return message;
    }
}

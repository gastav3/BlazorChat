
namespace BlazorChatShared.Models.Models;

public class ChatMessage
{
    public string Id { get; set; } = default!;
    public string User { get; set; } = default!;
    public string Message { get; set; } = default!;
    public string GroupId { get; set; } = default!;
    public DateTime Timestamp { get; set; }
}

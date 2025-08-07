using BlazorChatShared.Models.Models;

namespace BlazorChatShared.Parameters;

public class PagedMessagesResultParameter
{
    public IEnumerable<ChatMessage> Messages { get; set; } = [];
    public bool HasMore { get; set; }
}

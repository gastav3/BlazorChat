using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorChatShared.Models.Entities;

public class ChatMessageEntity
{
    public Guid Id { get; set; } = default!;
    public string User { get; set; } = default!;
    public string Message { get; set; } = default!;
    public Guid GroupId { get; set; } = default!;
    public DateTime Timestamp { get; set; }
}

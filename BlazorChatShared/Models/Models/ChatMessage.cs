
using System.ComponentModel.DataAnnotations;

namespace BlazorChatShared.Models.Models;

public class ChatMessage
{
    public string Id { get; set; } = default!;
    [Required(ErrorMessage = "Name is required")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Name must be 3–20 characters")]
    public string User { get; set; } = default!;
    [Required(ErrorMessage = "Message cannot be empty")]
    public string Message { get; set; } = default!;
    public string GroupId { get; set; } = default!;
    public DateTime Timestamp { get; set; }
}

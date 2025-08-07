using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorChatShared.Parameters;

public class CreateChatMessageParameter
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string User { get; set; } = default!;
    public string Message { get; set; } = default!;
    public Guid GroupId { get; set; } = default!;
    public DateTime SentAt { get; set; }
}

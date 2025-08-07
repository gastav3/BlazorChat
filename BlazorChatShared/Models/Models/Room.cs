using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorChatShared.Models.Models;

public class Room
{
    public string Id { get; set; } = default!;
    public string? Name { get; set; }
    public string? Owner { get; set; }
}

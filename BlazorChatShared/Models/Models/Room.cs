using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorChatShared.Models.Models;

public class Room
{
    public string Id { get; set; } = default!;
    [Required(ErrorMessage = "Name required")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Name must be 3–20 characters")]
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Owner { get; set; }
    public bool Hidden { get; set; }

    [NotMapped]
    public List<string> Connections { get; set; } = [];
}

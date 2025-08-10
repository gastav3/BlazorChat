using System.ComponentModel.DataAnnotations.Schema;
namespace BlazorChatShared.Models.Entities;

public class RoomEntity
{
    public Guid Id { get; set; } = default!;
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Owner { get; set; }
    public bool Hidden { get; set; }

    [NotMapped]
    public List<string> Connections { get; set; } = [];
}
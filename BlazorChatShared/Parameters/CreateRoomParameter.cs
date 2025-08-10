
namespace BlazorChatShared.Parameters;

public class CreateRoomParameter
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = default!;
    public string? Description { get; set; } = default!;
    // public string Owner { get; set; } = default!;
}

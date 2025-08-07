using Microsoft.EntityFrameworkCore;
using BlazorChatShared.Models.Entities;

namespace BlazorChatAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<RoomEntity> Rooms { get; set; } = default!;
    public DbSet<ChatMessageEntity> Messages { get; set; } = default!;
}

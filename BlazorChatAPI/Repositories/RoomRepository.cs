
using BlazorChatAPI.Data;
using BlazorChatShared.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlazorChatAPI.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly ApplicationDbContext _context;

    public RoomRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<RoomEntity>> GetAllRooms()
    {
        return await _context.Rooms.ToListAsync();
    }

    public async Task<RoomEntity?> GetRoomById(Guid id)
    {
        return await _context.Rooms.FindAsync(id);
    }

    public async Task<RoomEntity> CreateRoom(RoomEntity room)
    {
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync(); 
        return room;
    }
}

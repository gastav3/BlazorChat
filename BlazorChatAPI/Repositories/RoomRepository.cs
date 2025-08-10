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

    public async Task<List<RoomEntity>> GetAllRooms(bool showHidden = false)
    {
        return await _context.Rooms
            .Where(x => showHidden || !x.Hidden)
            .ToListAsync();
    }

    public async Task<RoomEntity?> GetRoomById(Guid id, bool showHidden = false)
    {
        return await _context.Rooms
            .Where(x => showHidden || !x.Hidden)
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<RoomEntity?> UpdateRoom(RoomEntity updatedRoom)
    {
        var existingRoom = await _context.Rooms.FindAsync(updatedRoom.Id);
        if (existingRoom == null)
        {
            return null;
        }

        var id = existingRoom.Id;
        _context.Entry(existingRoom).CurrentValues.SetValues(updatedRoom);
        existingRoom.Id = id;

        await _context.SaveChangesAsync();
        return existingRoom;
    }

    public async Task<RoomEntity> CreateRoom(RoomEntity room)
    {
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync(); 
        return room;
    }
}

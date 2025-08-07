using AutoMapper;
using BlazorChatAPI.Services;
using BlazorChatShared.Models.Entities;
using BlazorChatShared.Models.Models;
using BlazorChatShared.Parameters;
using Microsoft.AspNetCore.Mvc;

namespace BlazorChatAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatRoomController : ControllerBase
{
    private readonly IMapper _autoMapper;
    private readonly IRoomService _roomService;
    public ChatRoomController(IMapper autoMapper, IRoomService roomService)
    {
        _autoMapper = autoMapper ?? throw new ArgumentNullException(nameof(autoMapper));
        _roomService = roomService ?? throw new ArgumentNullException(nameof(roomService));
    }

    [HttpPost]
    public async Task<ActionResult<Room>> Create([FromBody] CreateRoomParameter request)
    {
        var roomEntity = new RoomEntity
        {
            Id = request.Id,
            Name = request.Name,
        };

        var createdRoom = await _roomService.CreateRoom(roomEntity);
        var room = _autoMapper.Map<Room>(createdRoom);

        return CreatedAtAction(nameof(Create), room.Id, room);
    }

    [HttpGet]
    public async Task<ActionResult<List<Room>>> GetAllRooms()
    {
        var roomsEntity = await _roomService.GetAllRooms();
        var rooms = _autoMapper.Map<IEnumerable<Room>>(roomsEntity);
        return Ok(rooms);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Room>> GetRoomById(string id)
    {
        var roomEntity = await _roomService.GetRoomById(id);
        var room = _autoMapper.Map<Room>(roomEntity);

        if (room == null)
        {
            return NotFound();
        }

        return Ok(room);
    }
}

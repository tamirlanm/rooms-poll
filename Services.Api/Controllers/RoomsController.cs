using Microsoft.AspNetCore.Mvc;
using Services.Api.DTOs;
using Services.Api.Interfaces;

namespace Services.Api.Controllers;
[ApiController]
[Route("api/rooms")]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;
    
    public RoomsController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpPost]
    public async Task<ActionResult<RoomResponse>> CreateRoom(CreateRoomRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Room name cannot be empty.");
        }

        var room = await _roomService.CreatRoomAsync(request);

        return CreatedAtAction(
            nameof(GetRoom),
            new {id = room.Id},    
            room);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RoomResponse>> GetRoom(Guid id)
    {
        var room = await _roomService.GetRoomAsync(id);

        if(room is null)
        {
            return NotFound();
        }   

        return Ok(room);
    }
}
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
    public ActionResult<RoomResponse> CreateRoom(CreateRoomRequset request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Room name cannot be empty.");
        }

        var room = _roomService.CreatRoom(request);

        return CreatedAtAction(
            nameof(GetRoom),
            new {Id = room.Id},    
            room);
    }

    [HttpGet("{id:guid}")]
    public ActionResult<RoomResponse> GetRoom(Guid id)
    {
        var room = _roomService.GetRoom(id);

        if(room is null)
        {
            return NotFound();
        }   

        return Ok(room);
    }
}
using System.Collections.Concurrent;
using Services.Api.DTOs;
using Services.Api.Interfaces;
using Services.Api.Models;

namespace Services.Api.Services;

public class RoomService : IRoomService
{
    private readonly ConcurrentDictionary<Guid, Room> _rooms = new();
    public RoomResponse CreatRoom(CreateRoomRequset request)
    {
        var room = new Room
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            CreatedAt = DateTime.UtcNow,
            MemberCount = 0
        };

        _rooms[room.Id] = room;
        return MapToResponse(room);
    }

    public RoomResponse? GetRoom(Guid id)
    {
        return _rooms.TryGetValue(id, out var room) ? MapToResponse(room) : null;
    }

    private static RoomResponse MapToResponse(Room room)
    {
        return new RoomResponse
        {
            Id = room.Id,
            Name = room.Name,
            CreatedAt = room.CreatedAt,
            MemberCount = room.MemberCount
        };
    }
}
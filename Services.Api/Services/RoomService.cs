using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Services.Api.Data;
using Services.Api.DTOs;
using Services.Api.Interfaces;
using Services.Api.Models;

namespace Services.Api.Services;

public class RoomService : IRoomService
{
    // private readonly ConcurrentDictionary<Guid, Room> _rooms = new();
    private readonly AppDbContext _dbContext;
    public RoomService(AppDbContext dbContext) { _dbContext = dbContext;}

    public async Task<RoomResponse> CreatRoomAsync(CreateRoomRequest request)
    {
        var room = new Room
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            CreatedAt = DateTime.UtcNow,
            MemberCount = 0
        };

        _dbContext.Add(room);
        await _dbContext.SaveChangesAsync();

        return MapToResponse(room);
    }

    public async Task<RoomResponse?> GetRoomAsync(Guid id)
    {
        var room = await _dbContext.Rooms.AsNoTracking().FirstOrDefaultAsync(room => room.Id == id);

        return room is null ? null : MapToResponse(room);
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
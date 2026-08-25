using Services.Api.DTOs;

namespace Services.Api.Interfaces;
public interface IRoomService
{
    Task<RoomResponse> CreatRoomAsync(CreateRoomRequest request);
    Task<RoomResponse?> GetRoomAsync(Guid id);
}
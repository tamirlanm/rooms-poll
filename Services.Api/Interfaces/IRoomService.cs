using Services.Api.DTOs;

namespace Services.Api.Interfaces;
public interface IRoomService
{
    RoomResponse CreatRoom(CreateRoomRequset request);
    RoomResponse? GetRoom(Guid id);
}
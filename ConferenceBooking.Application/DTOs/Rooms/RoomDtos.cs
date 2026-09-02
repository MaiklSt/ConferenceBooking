namespace ConferenceBooking.Application.DTOs.Rooms;

public record ServiceDto(string Name, decimal Price);

public record ServiceResponse(Guid Id, string Name, decimal Price);

public record CreateRoomRequest(
    string Name,
    int Capacity,
    decimal BaseHourlyRate,
    List<ServiceDto> Services);

public record UpdateRoomRequest(
    string Name,
    int Capacity,
    decimal BaseHourlyRate,
    List<ServiceDto>? Services);

public record RoomResponse(
    Guid Id,
    string Name,
    int Capacity,
    decimal BaseHourlyRate,
    List<ServiceResponse> Services);

public record SearchAvailableRoomsRequest(
    DateTime StartTime,
    DateTime EndTime,
    int MinCapacity);

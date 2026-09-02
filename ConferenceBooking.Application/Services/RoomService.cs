using ConferenceBooking.Application.DTOs.Rooms;
using ConferenceBooking.Application.Exceptions;
using ConferenceBooking.Application.Interfaces;
using ConferenceBooking.Domain.Entities;

namespace ConferenceBooking.Application.Services;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IBookingRepository _bookingRepository;

    public RoomService(IRoomRepository roomRepository, IBookingRepository bookingRepository)
    {
        _roomRepository = roomRepository;
        _bookingRepository = bookingRepository;
    }

    public async Task<IReadOnlyList<RoomResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var rooms = await _roomRepository.GetAllAsync(cancellationToken);
        return rooms.Select(MapToResponse).ToList();
    }

    public async Task<RoomResponse> CreateAsync(CreateRoomRequest request, CancellationToken cancellationToken = default)
    {
        var services = request.Services
            .Select(s => new Service(s.Name, s.Price))
            .ToList();

        var room = new Room(request.Name, request.Capacity, request.BaseHourlyRate, services);
        await _roomRepository.AddAsync(room, cancellationToken);

        return MapToResponse(room);
    }

    public async Task<RoomResponse> UpdateAsync(Guid id, UpdateRoomRequest request, CancellationToken cancellationToken = default)
    {
        var room = await _roomRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Зал з ID {id} не знайдено.");

        room.UpdateDetails(request.Name, request.Capacity, request.BaseHourlyRate);

        if (request.Services is not null)
        {
            ReplaceServices(room, request.Services);
        }

        await _roomRepository.UpdateAsync(room, cancellationToken);
        return MapToResponse(room);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var room = await _roomRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Зал з ID {id} не знайдено.");

        var bookings = await _bookingRepository.GetByRoomIdAsync(id, cancellationToken);
        if (bookings.Count > 0)
            throw new Domain.Exceptions.DomainException(
                $"Неможливо видалити зал «{room.Name}»: існують активні бронювання.");

        await _roomRepository.DeleteAsync(id, cancellationToken);
    }

    public async Task<IReadOnlyList<RoomResponse>> SearchAvailableAsync(
        SearchAvailableRoomsRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.EndTime <= request.StartTime)
            throw new Domain.Exceptions.DomainException("Час завершення має бути пізніше часу початку.");

        if (request.MinCapacity <= 0)
            throw new Domain.Exceptions.DomainException("Місткість має бути більше нуля.");

        var allRooms = await _roomRepository.GetAllAsync(cancellationToken);
        var suitableRooms = allRooms.Where(r => r.Capacity >= request.MinCapacity);
        var availableRooms = new List<Room>();

        foreach (var room in suitableRooms)
        {
            var bookings = await _bookingRepository.GetByRoomIdAsync(room.Id, cancellationToken);
            var isOccupied = bookings.Any(b =>
                b.StartTime < request.EndTime && b.EndTime > request.StartTime);

            if (!isOccupied)
                availableRooms.Add(room);
        }

        return availableRooms.Select(MapToResponse).ToList();
    }

    private static void ReplaceServices(Room room, List<ServiceDto> services)
    {
        foreach (var existing in room.AvailableServices.ToList())
            room.RemoveService(existing.Id);

        foreach (var dto in services)
            room.AddService(new Service(dto.Name, dto.Price));
    }

    private static RoomResponse MapToResponse(Room room) =>
        new(
            room.Id,
            room.Name,
            room.Capacity,
            room.BaseHourlyRate,
            room.AvailableServices
                .Select(s => new ServiceResponse(s.Id, s.Name, s.Price))
                .ToList());
}

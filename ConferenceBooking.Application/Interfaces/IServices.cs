using ConferenceBooking.Application.DTOs.Rooms;
using ConferenceBooking.Application.DTOs.Bookings;

namespace ConferenceBooking.Application.Interfaces;

public interface IRoomService
{
    Task<IReadOnlyList<RoomResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<RoomResponse> CreateAsync(CreateRoomRequest request, CancellationToken cancellationToken = default);
    Task<RoomResponse> UpdateAsync(Guid id, UpdateRoomRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RoomResponse>> SearchAvailableAsync(
        SearchAvailableRoomsRequest request,
        CancellationToken cancellationToken = default);
}

public interface IBookingService
{
    Task<BookingResponse> CreateAsync(CreateBookingRequest request, CancellationToken cancellationToken = default);
}

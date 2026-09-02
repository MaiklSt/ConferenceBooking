namespace ConferenceBooking.Application.DTOs.Bookings;

public record CreateBookingRequest(
    Guid RoomId,
    DateTime StartTime,
    TimeSpan Duration,
    List<Guid> SelectedServiceIds);

public record BookingResponse(
    Guid Id,
    Guid RoomId,
    DateTime StartTime,
    TimeSpan Duration,
    decimal TotalCost,
    List<Guid> SelectedServiceIds);

using ConferenceBooking.Application.DTOs.Bookings;
using ConferenceBooking.Application.Exceptions;
using ConferenceBooking.Application.Interfaces;
using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Domain.Exceptions;
using ConferenceBooking.Domain.Services;

namespace ConferenceBooking.Application.Services;

public class BookingService : IBookingService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly RentalCostCalculator _costCalculator;

    public BookingService(
        IRoomRepository roomRepository,
        IBookingRepository bookingRepository,
        RentalCostCalculator costCalculator)
    {
        _roomRepository = roomRepository;
        _bookingRepository = bookingRepository;
        _costCalculator = costCalculator;
    }

    public async Task<BookingResponse> CreateAsync(CreateBookingRequest request, CancellationToken cancellationToken = default)
    {
        var room = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken)
            ?? throw new NotFoundException($"Зал з ID {request.RoomId} не знайдено.");

        var selectedServices = ResolveSelectedServices(room, request.SelectedServiceIds);
        await EnsureRoomIsAvailableAsync(room.Id, request.StartTime, request.Duration, cancellationToken);

        var totalCost = _costCalculator.Calculate(
            room,
            request.StartTime,
            request.Duration,
            selectedServices);

        var booking = new Booking(
            room.Id,
            request.StartTime,
            request.Duration,
            request.SelectedServiceIds,
            totalCost);

        await _bookingRepository.AddAsync(booking, cancellationToken);

        return MapToResponse(booking);
    }

    private static List<Service> ResolveSelectedServices(Room room, List<Guid> selectedServiceIds)
    {
        var services = new List<Service>();

        foreach (var serviceId in selectedServiceIds)
        {
            var service = room.AvailableServices.FirstOrDefault(s => s.Id == serviceId)
                ?? throw new DomainException(
                    $"Послугу з ID {serviceId} не знайдено в залі «{room.Name}».");

            services.Add(service);
        }

        return services;
    }

    private async Task EnsureRoomIsAvailableAsync(
        Guid roomId,
        DateTime startTime,
        TimeSpan duration,
        CancellationToken cancellationToken)
    {
        var endTime = startTime.Add(duration);
        var bookings = await _bookingRepository.GetByRoomIdAsync(roomId, cancellationToken);

        if (bookings.Any(b => b.StartTime < endTime && b.EndTime > startTime))
            throw new DomainException("Зал вже заброньовано на обраний час.");
    }

    private static BookingResponse MapToResponse(Booking booking) =>
        new(
            booking.Id,
            booking.RoomId,
            booking.StartTime,
            booking.Duration,
            booking.TotalCost,
            booking.SelectedServiceIds.ToList());
}

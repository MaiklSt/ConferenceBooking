using ConferenceBooking.Application.DTOs.Reports;
using ConferenceBooking.Application.Interfaces;
using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Domain.Exceptions;

namespace ConferenceBooking.Application.Services;

/// <summary>
/// Звіти для бізнесу: завантаженість залів, виручка та популярні послуги.
/// </summary>
public class ReportService : IReportService
{
    private static readonly TimeSpan OperatingDayStart = new(6, 0, 0);
    private static readonly TimeSpan OperatingDayEnd = new(23, 0, 0);

    private readonly IRoomRepository _roomRepository;
    private readonly IBookingRepository _bookingRepository;

    public ReportService(IRoomRepository roomRepository, IBookingRepository bookingRepository)
    {
        _roomRepository = roomRepository;
        _bookingRepository = bookingRepository;
    }

    public async Task<IReadOnlyList<OccupancyReportItem>> GetOccupancyAsync(
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        var (periodStart, periodEnd) = NormalizePeriod(from, to);
        var rooms = await _roomRepository.GetAllAsync(cancellationToken);
        var bookings = await _bookingRepository.GetAllAsync(cancellationToken);
        var availableHours = CalculateAvailableHours(periodStart, periodEnd);

        return rooms
            .Select(room =>
            {
                var roomBookings = bookings.Where(b => b.RoomId == room.Id).ToList();
                var bookedHours = roomBookings
                    .Sum(b => GetOverlapHours(b, periodStart, periodEnd));

                var occupancy = availableHours == 0
                    ? 0
                    : Math.Round(bookedHours / availableHours * 100, 2);

                return new OccupancyReportItem(
                    room.Id,
                    room.Name,
                    roomBookings.Count(b => OverlapsPeriod(b, periodStart, periodEnd)),
                    Math.Round(bookedHours, 2),
                    availableHours,
                    occupancy);
            })
            .ToList();
    }

    public async Task<IReadOnlyList<RevenueReportItem>> GetRevenueAsync(
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        var (periodStart, periodEnd) = NormalizePeriod(from, to);
        var rooms = await _roomRepository.GetAllAsync(cancellationToken);
        var bookings = await _bookingRepository.GetAllAsync(cancellationToken);

        return rooms
            .Select(room =>
            {
                var periodBookings = bookings
                    .Where(b => b.RoomId == room.Id && OverlapsPeriod(b, periodStart, periodEnd))
                    .ToList();

                return new RevenueReportItem(
                    room.Id,
                    room.Name,
                    periodBookings.Count,
                    periodBookings.Sum(b => b.TotalCost));
            })
            .OrderByDescending(item => item.TotalRevenue)
            .ToList();
    }

    public async Task<IReadOnlyList<PopularServiceReportItem>> GetPopularServicesAsync(
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        var (periodStart, periodEnd) = NormalizePeriod(from, to);
        var rooms = await _roomRepository.GetAllAsync(cancellationToken);
        var bookings = await _bookingRepository.GetAllAsync(cancellationToken);
        var servicesById = rooms
            .SelectMany(r => r.AvailableServices)
            .GroupBy(s => s.Id)
            .ToDictionary(g => g.Key, g => g.First());

        return bookings
            .Where(b => OverlapsPeriod(b, periodStart, periodEnd))
            .SelectMany(b => b.SelectedServiceIds)
            .Where(id => servicesById.ContainsKey(id))
            .GroupBy(id => servicesById[id].Name, StringComparer.OrdinalIgnoreCase)
            .Select(group =>
            {
                var sample = servicesById.Values.First(s =>
                    s.Name.Equals(group.Key, StringComparison.OrdinalIgnoreCase));

                return new PopularServiceReportItem(
                    sample.Name,
                    group.Count(),
                    group.Count() * sample.Price);
            })
            .OrderByDescending(item => item.TimesBooked)
            .ThenBy(item => item.ServiceName)
            .ToList();
    }

    public async Task<ReportsSummary> GetSummaryAsync(
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        var (periodStart, periodEnd) = NormalizePeriod(from, to);
        var occupancy = await GetOccupancyAsync(periodStart, periodEnd, cancellationToken);
        var revenue = await GetRevenueAsync(periodStart, periodEnd, cancellationToken);
        var services = await GetPopularServicesAsync(periodStart, periodEnd, cancellationToken);

        var averageOccupancy = occupancy.Count == 0
            ? 0
            : Math.Round(occupancy.Average(item => item.OccupancyPercent), 2);

        return new ReportsSummary(
            periodStart,
            periodEnd,
            revenue.Sum(item => item.BookingsCount),
            revenue.Sum(item => item.TotalRevenue),
            averageOccupancy,
            occupancy,
            revenue,
            services);
    }

    private static (DateTime From, DateTime To) NormalizePeriod(DateTime from, DateTime to)
    {
        if (to <= from)
            throw new DomainException("Дата завершення звіту має бути пізніше дати початку.");

        return (from, to);
    }

    private static bool OverlapsPeriod(Booking booking, DateTime from, DateTime to) =>
        booking.StartTime < to && booking.EndTime > from;

    private static decimal GetOverlapHours(Booking booking, DateTime from, DateTime to)
    {
        var overlapStart = booking.StartTime > from ? booking.StartTime : from;
        var overlapEnd = booking.EndTime < to ? booking.EndTime : to;

        if (overlapEnd <= overlapStart)
            return 0;

        return (decimal)(overlapEnd - overlapStart).TotalHours;
    }

    private static decimal CalculateAvailableHours(DateTime from, DateTime to)
    {
        var hours = 0m;
        var day = from.Date;

        while (day < to)
        {
            var dayStart = day.Add(OperatingDayStart);
            var dayEnd = day.Add(OperatingDayEnd);
            var overlapStart = from > dayStart ? from : dayStart;
            var overlapEnd = to < dayEnd ? to : dayEnd;

            if (overlapEnd > overlapStart)
                hours += (decimal)(overlapEnd - overlapStart).TotalHours;

            day = day.AddDays(1);
        }

        return Math.Round(hours, 2);
    }
}

namespace ConferenceBooking.Application.DTOs.Reports;

public record OccupancyReportItem(
    Guid RoomId,
    string RoomName,
    int BookingsCount,
    decimal BookedHours,
    decimal AvailableHours,
    decimal OccupancyPercent);

public record RevenueReportItem(
    Guid RoomId,
    string RoomName,
    int BookingsCount,
    decimal TotalRevenue);

public record PopularServiceReportItem(
    string ServiceName,
    int TimesBooked,
    decimal TotalRevenue);

public record ReportsSummary(
    DateTime From,
    DateTime To,
    int TotalBookings,
    decimal TotalRevenue,
    decimal AverageOccupancyPercent,
    IReadOnlyList<OccupancyReportItem> Occupancy,
    IReadOnlyList<RevenueReportItem> Revenue,
    IReadOnlyList<PopularServiceReportItem> PopularServices);

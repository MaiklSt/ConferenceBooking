using ConferenceBooking.Application.DTOs.Reports;

namespace ConferenceBooking.Application.Interfaces;

public interface IReportService
{
    Task<IReadOnlyList<OccupancyReportItem>> GetOccupancyAsync(
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RevenueReportItem>> GetRevenueAsync(
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PopularServiceReportItem>> GetPopularServicesAsync(
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default);

    Task<ReportsSummary> GetSummaryAsync(
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default);
}

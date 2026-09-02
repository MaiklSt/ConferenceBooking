using ConferenceBooking.Application.DTOs.Reports;
using ConferenceBooking.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceBooking.API.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>
    /// Зведення: завантаженість, виручка та популярні послуги за період.
    /// </summary>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(ReportsSummary), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ReportsSummary>> GetSummary(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken cancellationToken)
    {
        var report = await _reportService.GetSummaryAsync(from, to, cancellationToken);
        return Ok(report);
    }

    /// <summary>
    /// Завантаженість залів за період (відсоток зайнятих робочих годин 06:00–23:00).
    /// </summary>
    [HttpGet("occupancy")]
    [ProducesResponseType(typeof(IReadOnlyList<OccupancyReportItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<OccupancyReportItem>>> GetOccupancy(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken cancellationToken)
    {
        var report = await _reportService.GetOccupancyAsync(from, to, cancellationToken);
        return Ok(report);
    }

    /// <summary>
    /// Виручка по залах за період.
    /// </summary>
    [HttpGet("revenue")]
    [ProducesResponseType(typeof(IReadOnlyList<RevenueReportItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<RevenueReportItem>>> GetRevenue(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken cancellationToken)
    {
        var report = await _reportService.GetRevenueAsync(from, to, cancellationToken);
        return Ok(report);
    }

    /// <summary>
    /// Популярні додаткові послуги за період.
    /// </summary>
    [HttpGet("services")]
    [ProducesResponseType(typeof(IReadOnlyList<PopularServiceReportItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<PopularServiceReportItem>>> GetPopularServices(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken cancellationToken)
    {
        var report = await _reportService.GetPopularServicesAsync(from, to, cancellationToken);
        return Ok(report);
    }
}

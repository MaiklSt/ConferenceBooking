using ConferenceBooking.Application.DTOs.Rooms;
using ConferenceBooking.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceBooking.API.Controllers;

[ApiController]
[Route("api/rooms")]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomsController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    /// <summary>
    /// Отримати всі конференц-зали (потрібно, щоб взяти ID послуг для бронювання).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<RoomResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<RoomResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var rooms = await _roomService.GetAllAsync(cancellationToken);
        return Ok(rooms);
    }

    /// <summary>
    /// Знайти вільні зали за датою, часом і мінімальною місткістю.
    /// </summary>
    [HttpGet("available")]
    [ProducesResponseType(typeof(IReadOnlyList<RoomResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<RoomResponse>>> SearchAvailable(
        [FromQuery] DateTime startTime,
        [FromQuery] DateTime endTime,
        [FromQuery] int minCapacity,
        CancellationToken cancellationToken)
    {
        var rooms = await _roomService.SearchAvailableAsync(
            new SearchAvailableRoomsRequest(startTime, endTime, minCapacity),
            cancellationToken);

        return Ok(rooms);
    }

    /// <summary>
    /// Створити новий конференц-зал.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(RoomResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RoomResponse>> Create(
        [FromBody] CreateRoomRequest request,
        CancellationToken cancellationToken)
    {
        var room = await _roomService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Create), new { id = room.Id }, room);
    }

    /// <summary>
    /// Оновити інформацію про конференц-зал.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(RoomResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RoomResponse>> Update(
        Guid id,
        [FromBody] UpdateRoomRequest request,
        CancellationToken cancellationToken)
    {
        var room = await _roomService.UpdateAsync(id, request, cancellationToken);
        return Ok(room);
    }

    /// <summary>
    /// Видалити конференц-зал.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _roomService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}

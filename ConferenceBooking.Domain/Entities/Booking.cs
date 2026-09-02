namespace ConferenceBooking.Domain.Entities;

/// <summary>
/// Бронювання конференц-залу на певний період часу.
/// </summary>
public class Booking
{
    public Guid Id { get; private set; }
    public Guid RoomId { get; private set; }
    public DateTime StartTime { get; private set; }
    public TimeSpan Duration { get; private set; }
    public decimal TotalCost { get; private set; }

    private readonly List<Guid> _selectedServiceIds = [];
    public IReadOnlyCollection<Guid> SelectedServiceIds => _selectedServiceIds.AsReadOnly();

    private Booking() { }

    public Booking(
        Guid roomId,
        DateTime startTime,
        TimeSpan duration,
        IEnumerable<Guid> selectedServiceIds,
        decimal totalCost)
    {
        if (roomId == Guid.Empty)
            throw new ArgumentException("RoomId не може бути порожнім.", nameof(roomId));

        if (duration <= TimeSpan.Zero)
            throw new ArgumentException("Тривалість має бути більше нуля.", nameof(duration));

        if (totalCost < 0)
            throw new ArgumentException("Вартість не може бути від'ємною.", nameof(totalCost));

        Id = Guid.NewGuid();
        RoomId = roomId;
        StartTime = startTime;
        Duration = duration;
        TotalCost = totalCost;
        _selectedServiceIds.AddRange(selectedServiceIds);
    }

    public DateTime EndTime => StartTime.Add(Duration);

    /// <summary>
    /// Перевіряє, чи перетинається це бронювання з іншим у тому самому залі.
    /// </summary>
    public bool OverlapsWith(Booking other)
    {
        if (RoomId != other.RoomId)
            return false;

        return StartTime < other.EndTime && EndTime > other.StartTime;
    }
}

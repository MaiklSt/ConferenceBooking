namespace ConferenceBooking.Application.Interfaces;

public interface IBookingRepository
{
    Task<Domain.Entities.Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Domain.Entities.Booking>> GetByRoomIdAsync(Guid roomId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Domain.Entities.Booking>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Domain.Entities.Booking booking, CancellationToken cancellationToken = default);
}

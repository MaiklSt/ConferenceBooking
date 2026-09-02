namespace ConferenceBooking.Application.Interfaces;

public interface IRoomRepository
{
    Task<Domain.Entities.Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Domain.Entities.Room>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Domain.Entities.Room room, CancellationToken cancellationToken = default);
    Task UpdateAsync(Domain.Entities.Room room, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

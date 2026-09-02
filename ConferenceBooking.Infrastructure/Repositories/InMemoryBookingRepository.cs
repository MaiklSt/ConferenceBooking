using ConferenceBooking.Application.Interfaces;
using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Infrastructure.Persistence;

namespace ConferenceBooking.Infrastructure.Repositories;

public class InMemoryBookingRepository : IBookingRepository
{
    private readonly InMemoryDataStore _store;

    public InMemoryBookingRepository(InMemoryDataStore store)
    {
        _store = store;
    }

    public Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_store.SyncRoot)
        {
            return Task.FromResult(_store.Bookings.FirstOrDefault(b => b.Id == id));
        }
    }

    public Task<IReadOnlyList<Booking>> GetByRoomIdAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        lock (_store.SyncRoot)
        {
            IReadOnlyList<Booking> bookings = _store.Bookings
                .Where(b => b.RoomId == roomId)
                .ToList();

            return Task.FromResult(bookings);
        }
    }

    public Task<IReadOnlyList<Booking>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        lock (_store.SyncRoot)
        {
            IReadOnlyList<Booking> bookings = _store.Bookings.ToList();
            return Task.FromResult(bookings);
        }
    }

    public Task AddAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        lock (_store.SyncRoot)
        {
            _store.Bookings.Add(booking);
        }

        return Task.CompletedTask;
    }
}

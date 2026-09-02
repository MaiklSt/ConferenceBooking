using ConferenceBooking.Application.Interfaces;
using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Infrastructure.Persistence;

namespace ConferenceBooking.Infrastructure.Repositories;

public class InMemoryRoomRepository : IRoomRepository
{
    private readonly InMemoryDataStore _store;

    public InMemoryRoomRepository(InMemoryDataStore store)
    {
        _store = store;
    }

    public Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_store.SyncRoot)
        {
            return Task.FromResult(_store.Rooms.FirstOrDefault(r => r.Id == id));
        }
    }

    public Task<IReadOnlyList<Room>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        lock (_store.SyncRoot)
        {
            IReadOnlyList<Room> rooms = _store.Rooms.ToList();
            return Task.FromResult(rooms);
        }
    }

    public Task AddAsync(Room room, CancellationToken cancellationToken = default)
    {
        lock (_store.SyncRoot)
        {
            _store.Rooms.Add(room);
        }

        return Task.CompletedTask;
    }

    public Task UpdateAsync(Room room, CancellationToken cancellationToken = default)
    {
        lock (_store.SyncRoot)
        {
            var exists = _store.Rooms.Any(r => r.Id == room.Id);
            if (!exists)
                throw new KeyNotFoundException($"Зал з ID {room.Id} не знайдено.");
        }

        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_store.SyncRoot)
        {
            _store.Rooms.RemoveAll(r => r.Id == id);
        }

        return Task.CompletedTask;
    }
}

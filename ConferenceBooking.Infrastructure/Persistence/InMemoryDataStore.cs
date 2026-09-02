using ConferenceBooking.Domain.Entities;

namespace ConferenceBooking.Infrastructure.Persistence;

/// <summary>
/// In-memory сховище даних. Реєструється як Singleton — дані живуть протягом роботи застосунку.
/// </summary>
public class InMemoryDataStore
{
    internal readonly object SyncRoot = new();
    internal readonly List<Room> Rooms = [];
    internal readonly List<Booking> Bookings = [];
}

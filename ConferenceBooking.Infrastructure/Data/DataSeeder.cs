using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Infrastructure.Persistence;

namespace ConferenceBooking.Infrastructure.Data;

/// <summary>
/// Заповнює сховище початковими даними з технічного завдання.
/// </summary>
public static class DataSeeder
{
    public static void Seed(InMemoryDataStore store)
    {
        lock (store.SyncRoot)
        {
            if (store.Rooms.Count > 0)
                return;

            store.Rooms.Add(CreateRoom("Зал A", 50, 2000));
            store.Rooms.Add(CreateRoom("Зал B", 100, 3500));
            store.Rooms.Add(CreateRoom("Зал C", 30, 1500));
        }
    }

    private static Room CreateRoom(string name, int capacity, decimal baseHourlyRate)
    {
        var services = new List<Service>
        {
            new("Проєктор", 500),
            new("Wi-Fi", 300),
            new("Звук", 700)
        };

        return new Room(name, capacity, baseHourlyRate, services);
    }
}

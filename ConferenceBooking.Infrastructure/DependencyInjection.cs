using ConferenceBooking.Application.Interfaces;
using ConferenceBooking.Infrastructure.Data;
using ConferenceBooking.Infrastructure.Persistence;
using ConferenceBooking.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace ConferenceBooking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<InMemoryDataStore>(_ =>
        {
            var store = new InMemoryDataStore();
            DataSeeder.Seed(store);
            return store;
        });

        services.AddScoped<IRoomRepository, InMemoryRoomRepository>();
        services.AddScoped<IBookingRepository, InMemoryBookingRepository>();

        return services;
    }
}

using Microsoft.OpenApi.Models;

namespace ConferenceBooking.API.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddApiSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Conference Booking API",
                Version = "v1",
                Description = """
                    API для управління конференц-залами, бронюваннями та розрахунком вартості оренди.

                    Робочі години: 06:00–23:00.
                    Тарифи: ранок −10%, стандарт, пік (12:00–14:00) +15%, вечір −20%.
                    """
            });

            var xmlPath = Path.Combine(AppContext.BaseDirectory, "ConferenceBooking.API.xml");
            if (File.Exists(xmlPath))
                options.IncludeXmlComments(xmlPath);
        });

        return services;
    }

    public static WebApplication UseApiSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Conference Booking API v1");
            options.RoutePrefix = "swagger";
        });

        return app;
    }
}

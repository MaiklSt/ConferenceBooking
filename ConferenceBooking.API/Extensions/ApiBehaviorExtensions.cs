using ConferenceBooking.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceBooking.API.Extensions;

public static class ApiBehaviorExtensions
{
    public static IServiceCollection ConfigureInvalidModelStateResponse(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var details = context.ModelState
                    .Where(entry => entry.Value is { Errors.Count: > 0 })
                    .SelectMany(entry => entry.Value!.Errors.Select(error =>
                        string.IsNullOrWhiteSpace(error.ErrorMessage)
                            ? $"{entry.Key}: некоректне значення."
                            : error.ErrorMessage))
                    .Distinct()
                    .ToList();

                var body = new ErrorResponse(
                    StatusCodes.Status400BadRequest,
                    "Запит не пройшов валідацію.",
                    details);

                return new BadRequestObjectResult(body);
            };
        });

        return services;
    }
}

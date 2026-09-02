using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Domain.Exceptions;

namespace ConferenceBooking.Domain.Services;

/// <summary>
/// Розраховує загальну вартість оренди залу з урахуванням часу доби та обраних послуг.
/// </summary>
public class RentalCostCalculator
{
    /// <summary>
    /// Розраховує вартість оренди: ціна залу (погодинно з коефіцієнтами) + одноразова ціна послуг.
    /// </summary>
    public decimal Calculate(
        Room room,
        DateTime startTime,
        TimeSpan duration,
        IEnumerable<Service> selectedServices)
    {
        ArgumentNullException.ThrowIfNull(room);
        ArgumentNullException.ThrowIfNull(selectedServices);

        ValidateBookingTime(startTime, duration);

        var roomCost = CalculateRoomCost(room.BaseHourlyRate, startTime, duration);
        var servicesCost = selectedServices.Sum(s => s.Price);

        return Math.Round(roomCost + servicesCost, 2);
    }

    private static void ValidateBookingTime(DateTime startTime, TimeSpan duration)
    {
        if (duration <= TimeSpan.Zero)
            throw new DomainException("Тривалість бронювання має бути більше нуля.");

        var endTime = startTime.Add(duration);
        var dayEnd = startTime.Date.Add(new TimeSpan(23, 0, 0));

        if (startTime.TimeOfDay < new TimeSpan(6, 0, 0))
            throw new DomainException(
                $"Час початку {startTime:HH:mm} поза робочими годинами (06:00–23:00).");

        if (endTime > dayEnd)
            throw new DomainException(
                $"Час завершення {endTime:HH:mm} поза робочими годинами (06:00–23:00).");
    }

    /// <summary>
    /// Рахує вартість залу похвилинно, застосовуючи коефіцієнт для кожної хвилини.
    /// </summary>
    private static decimal CalculateRoomCost(decimal baseHourlyRate, DateTime startTime, TimeSpan duration)
    {
        decimal total = 0;
        var endTime = startTime.Add(duration);
        var current = startTime;

        while (current < endTime)
        {
            var period = TimePeriodResolver.Resolve(current.TimeOfDay);
            var multiplier = TimePeriodResolver.GetMultiplier(period);
            var ratePerMinute = baseHourlyRate / 60m * multiplier;

            total += ratePerMinute;
            current = current.AddMinutes(1);
        }

        return total;
    }
}

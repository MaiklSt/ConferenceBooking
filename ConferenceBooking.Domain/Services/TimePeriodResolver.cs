using ConferenceBooking.Domain.Enums;

namespace ConferenceBooking.Domain.Services;

/// <summary>
/// Визначає період доби та коефіцієнт ціни для конкретного моменту часу.
/// </summary>
public static class TimePeriodResolver
{
    private static readonly TimeSpan MorningStart = new(6, 0, 0);
    private static readonly TimeSpan MorningEnd = new(9, 0, 0);
    private static readonly TimeSpan PeakStart = new(12, 0, 0);
    private static readonly TimeSpan PeakEnd = new(14, 0, 0);
    private static readonly TimeSpan StandardEnd = new(18, 0, 0);
    private static readonly TimeSpan EveningEnd = new(23, 0, 0);

    public static TimePeriod Resolve(TimeSpan timeOfDay)
    {
        if (timeOfDay >= PeakStart && timeOfDay < PeakEnd)
            return TimePeriod.Peak;

        if (timeOfDay >= MorningStart && timeOfDay < MorningEnd)
            return TimePeriod.Morning;

        if (timeOfDay >= MorningEnd && timeOfDay < StandardEnd)
            return TimePeriod.Standard;

        if (timeOfDay >= StandardEnd && timeOfDay < EveningEnd)
            return TimePeriod.Evening;

        throw new Exceptions.DomainException(
            $"Час {timeOfDay:hh\\:mm} поза робочими годинами (06:00–23:00).");
    }

    public static decimal GetMultiplier(TimePeriod period) => period switch
    {
        TimePeriod.Morning => 0.90m,   // знижка 10%
        TimePeriod.Standard => 1.00m,
        TimePeriod.Peak => 1.15m,      // націнка 15%
        TimePeriod.Evening => 0.80m,   // знижка 20%
        _ => throw new ArgumentOutOfRangeException(nameof(period), period, null)
    };

}

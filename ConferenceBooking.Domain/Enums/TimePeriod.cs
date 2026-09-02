namespace ConferenceBooking.Domain.Enums;

/// <summary>
/// Період доби для розрахунку вартості оренди.
/// </summary>
public enum TimePeriod
{
    Morning,   // 06:00–09:00, знижка 10%
    Standard,  // 09:00–18:00, базова ціна
    Peak,      // 12:00–14:00, націнка 15%
    Evening    // 18:00–23:00, знижка 20%
}

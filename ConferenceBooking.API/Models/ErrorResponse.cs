namespace ConferenceBooking.API.Models;

public record ErrorResponse(int Status, string Error, IReadOnlyList<string>? Details = null);

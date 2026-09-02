using ConferenceBooking.Application.DTOs.Bookings;
using FluentValidation;

namespace ConferenceBooking.Application.Validators;

public class CreateBookingRequestValidator : AbstractValidator<CreateBookingRequest>
{
    public CreateBookingRequestValidator()
    {
        RuleFor(x => x.RoomId)
            .NotEmpty().WithMessage("ID залу є обов'язковим.");

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Час початку бронювання є обов'язковим.");

        RuleFor(x => x.Duration)
            .GreaterThan(TimeSpan.Zero).WithMessage("Тривалість має бути більше нуля.")
            .LessThanOrEqualTo(TimeSpan.FromHours(17))
            .WithMessage("Тривалість не може перевищувати робочий день (06:00–23:00).");

        RuleFor(x => x.SelectedServiceIds)
            .NotNull().WithMessage("Список послуг є обов'язковим.");
    }
}

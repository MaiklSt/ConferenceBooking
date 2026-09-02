using ConferenceBooking.Application.DTOs.Rooms;
using FluentValidation;

namespace ConferenceBooking.Application.Validators;

public class CreateRoomRequestValidator : AbstractValidator<CreateRoomRequest>
{
    public CreateRoomRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Назва залу є обов'язковою.")
            .MaximumLength(100).WithMessage("Назва залу не може бути довшою за 100 символів.");

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Місткість має бути більше нуля.");

        RuleFor(x => x.BaseHourlyRate)
            .GreaterThan(0).WithMessage("Базова вартість має бути більше нуля.");

        RuleFor(x => x.Services)
            .NotNull().WithMessage("Список послуг є обов'язковим.");

        RuleForEach(x => x.Services)
            .SetValidator(new ServiceDtoValidator());
    }
}

public class UpdateRoomRequestValidator : AbstractValidator<UpdateRoomRequest>
{
    public UpdateRoomRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Назва залу є обов'язковою.")
            .MaximumLength(100).WithMessage("Назва залу не може бути довшою за 100 символів.");

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Місткість має бути більше нуля.");

        RuleFor(x => x.BaseHourlyRate)
            .GreaterThan(0).WithMessage("Базова вартість має бути більше нуля.");

        RuleForEach(x => x.Services)
            .SetValidator(new ServiceDtoValidator())
            .When(x => x.Services is not null);
    }
}

public class ServiceDtoValidator : AbstractValidator<ServiceDto>
{
    public ServiceDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Назва послуги є обов'язковою.")
            .MaximumLength(100).WithMessage("Назва послуги не може бути довшою за 100 символів.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Вартість послуги має бути більше нуля.");
    }
}

public class SearchAvailableRoomsRequestValidator : AbstractValidator<SearchAvailableRoomsRequest>
{
    public SearchAvailableRoomsRequestValidator()
    {
        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Час початку є обов'язковим.");

        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime)
            .WithMessage("Час завершення має бути пізніше часу початку.");

        RuleFor(x => x.MinCapacity)
            .GreaterThan(0).WithMessage("Місткість має бути більше нуля.");
    }
}

using FluentValidation;

namespace Contract.Dtos.Schedule.Requests;

public class UpdateScheduleSettingsRequest
{
    public Guid MentorId { get; set; }
    public DateOnly WeekStartDate { get; set; }
    public DateOnly WeekEndDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int SessionDuration { get; set; }
    public int BufferTime { get; set; }
}

public class CreateScheduleSettingsRequestValidator : AbstractValidator<UpdateScheduleSettingsRequest>
{
    public CreateScheduleSettingsRequestValidator()
    {
        RuleFor(x => x.WeekStartDate)
            .NotEmpty().WithMessage("WeekStartDate is required.")
            .Must(date => date.DayOfWeek == DayOfWeek.Sunday)
            .WithMessage("WeekStartDate must be a Sunday.");

        RuleFor(x => x.WeekEndDate)
            .NotEmpty().WithMessage("WeekEndDate is required.")
            .Must(date => date.DayOfWeek == DayOfWeek.Saturday)
            .WithMessage("WeekEndDate must be a Saturday.");

        When(x => x.WeekStartDate != default && x.WeekEndDate != default, () =>
        {
            RuleFor(x => x.WeekEndDate)
                .Must((request, weekEndDate) => weekEndDate.ToDateTime(TimeOnly.MinValue) - request.WeekStartDate.ToDateTime(TimeOnly.MinValue) == TimeSpan.FromDays(6))
                .WithMessage("The difference between WeekEndDate and WeekStartDate must be 6 days.");
        });

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("StartTime is required.");

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("EndTime is required.")
            .GreaterThan(x => x.StartTime).WithMessage("EndTime must be greater than StartTime.");

        RuleFor(x => x.SessionDuration)
            .InclusiveBetween(30, 90).WithMessage("SessionDuration must be between 30 and 90 minutes.");

        RuleFor(x => x.BufferTime)
            .InclusiveBetween(0, 60).WithMessage("BufferTime must be greater than or equal to 0 minutes.");
    }
}
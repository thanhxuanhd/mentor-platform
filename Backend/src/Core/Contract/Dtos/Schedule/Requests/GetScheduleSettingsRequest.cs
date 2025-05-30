using FluentValidation;

namespace Contract.Dtos.Schedule.Requests;

public class GetScheduleSettingsRequest
{
    public DateOnly? WeekStartDate { get; set; }
    public DateOnly? WeekEndDate { get; set; }
}

public class GetScheduleSettingsRequestValidator : AbstractValidator<GetScheduleSettingsRequest>
{
    public GetScheduleSettingsRequestValidator()
    {
        RuleFor(x => x.WeekStartDate)
            .Must(date => !date.HasValue || date.Value.DayOfWeek == DayOfWeek.Sunday)
            .WithMessage("WeekStartDate must be a Sunday.");

        RuleFor(x => x.WeekEndDate)
            .Must(date => !date.HasValue || date.Value.DayOfWeek == DayOfWeek.Saturday)
            .WithMessage("WeekEndDate must be a Saturday.");

        // TODO: The logic could be wrong, I didn't tested this
        When(x => x.WeekStartDate.HasValue && x.WeekEndDate.HasValue, () =>
        {
            RuleFor(x => x.WeekEndDate)
                .Must((request, weekEndDate) => weekEndDate!.Value.ToDateTime(TimeOnly.MinValue) - request.WeekStartDate!.Value.ToDateTime(TimeOnly.MinValue) == TimeSpan.FromDays(6))
                .WithMessage("The difference between WeekEndDate and WeekStartDate must be 6 days.");
        });
    }
}

using Contract.Dtos.Schedule.Extensions;
using FluentValidation;

namespace Contract.Dtos.Schedule.Requests;

public class SaveScheduleSettingsRequest
{
    public DateOnly WeekStartDate { get; set; }
    public DateOnly WeekEndDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int SessionDuration { get; set; }
    public int BufferTime { get; set; }
    public Dictionary<DateOnly, List<TimeSlotRequest>> AvailableTimeSlots { get; set; } = new Dictionary<DateOnly, List<TimeSlotRequest>>();
}

public class UpdateScheduleSettingsRequestValidator : AbstractValidator<SaveScheduleSettingsRequest>
{
    public UpdateScheduleSettingsRequestValidator()
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

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("EndTime is required.")
            .GreaterThan(x => x.StartTime).WithMessage("EndTime must be greater than StartTime.");

        RuleFor(x => x.SessionDuration)
            .InclusiveBetween(30, 90).WithMessage("SessionDuration must be between 30 and 90 minutes.");

        RuleFor(x => x.BufferTime)
            .InclusiveBetween(0, 60).WithMessage("BufferTime must be greater than or equal to 0 minutes.");

        RuleFor(x => x.AvailableTimeSlots)
            .NotNull().WithMessage("AvailableTimeSlots cannot be null.")
            .Custom((availableTimeSlots, context) =>
            {
                var request = (SaveScheduleSettingsRequest)context.InstanceToValidate;

                foreach (var kvp in availableTimeSlots)
                {
                    var date = kvp.Key;
                    var slotsForDate = kvp.Value;

                    // 1. Validate Date Key
                    if (date < request.WeekStartDate || date > request.WeekEndDate)
                    {
                        context.AddFailure(new FluentValidation.Results.ValidationFailure($"{context.PropertyPath}[{date}]", $"The date {date} is outside the specified week range ({request.WeekStartDate} to {request.WeekEndDate})."));
                    }

                    if (!slotsForDate.Any())
                    {
                        continue; // Empty list for a date is acceptable
                    }

                    var sortedSlots = slotsForDate.Where(s => s != null).OrderBy(s => s.StartTime).ToList();

                    for (int i = 0; i < sortedSlots.Count; i++)
                    {
                        var currentSlot = sortedSlots[i];

                        if (currentSlot.StartTime >= currentSlot.EndTime)
                        {
                            context.AddFailure(new FluentValidation.Results.ValidationFailure($"{context.PropertyPath}[{date}][{i}]", $"For date {date}, slot EndTime ({currentSlot.EndTime:HH:mm}) must be after StartTime ({currentSlot.StartTime:HH:mm})."));
                        }

                        if ((currentSlot.EndTime - currentSlot.StartTime).TotalMinutes != request.SessionDuration)
                        {
                            context.AddFailure(new FluentValidation.Results.ValidationFailure($"{context.PropertyPath}[{date}][{i}]", $"For date {date}, slot {currentSlot.StartTime:HH:mm}-{currentSlot.EndTime:HH:mm} duration must be {request.SessionDuration} minutes."));
                        }

                        if (currentSlot.StartTime < request.StartTime || currentSlot.EndTime > request.EndTime)
                        {
                            context.AddFailure(new FluentValidation.Results.ValidationFailure($"{context.PropertyPath}[{date}][{i}]", $"For date {date}, slot {currentSlot.StartTime:HH:mm}-{currentSlot.EndTime:HH:mm} must be within the daily schedule bounds ({request.StartTime:HH:mm} - {request.EndTime:HH:mm})."));
                        }

                        if (i < sortedSlots.Count - 1 && currentSlot.EndTime > sortedSlots[i + 1].StartTime)
                        {
                            context.AddFailure(new FluentValidation.Results.ValidationFailure($"{context.PropertyPath}[{date}]", $"Time slots on {date} overlap: specifically {currentSlot.StartTime:HH:mm}-{currentSlot.EndTime:HH:mm} and {sortedSlots[i + 1].StartTime:HH:mm}-{sortedSlots[i + 1].EndTime:HH:mm}."));
                        }
                    }
                }
            });
    }
}
using FluentValidation;

namespace Contract.Dtos.Categories.Requests;

public class FilterCourseByCategoryRequest
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class FilterCourseByCategoryRequestValidator : AbstractValidator<FilterCourseByCategoryRequest>
{
    public FilterCourseByCategoryRequestValidator()
    {
        RuleFor(x => x.PageIndex)
            .GreaterThan(0)
            .WithMessage("Page index must be greater than 0.");
        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Page size must be greater than 0.");
    }
}

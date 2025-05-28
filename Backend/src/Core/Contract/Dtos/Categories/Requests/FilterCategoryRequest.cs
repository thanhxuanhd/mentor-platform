using FluentValidation;

namespace Contract.Dtos.Categories.Requests;

public class FilterCategoryRequest
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 5;
    public string? Keyword { get; set; }
    public bool? Status { get; set; }
}

public class FilterCategoryRequestValidator : AbstractValidator<FilterCategoryRequest>
{
    public FilterCategoryRequestValidator()
    {
        RuleFor(x => x.PageIndex)
            .GreaterThan(0)
            .WithMessage("Page index must be greater than 0");
        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Page size must be greater than 0");
    }
}
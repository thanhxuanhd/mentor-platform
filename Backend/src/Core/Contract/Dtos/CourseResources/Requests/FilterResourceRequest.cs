using Domain.Enums;
using FluentValidation;

namespace Contract.Dtos.CourseResources.Requests
{
    public class FilterResourceRequest
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 8;
        public string? Keyword { get; set; }
        public FileType? ResourceType { get; set; }
    }
    public class FilterResourceRequestValidator : AbstractValidator<FilterResourceRequest>
    {
        public FilterResourceRequestValidator()
        {
            RuleFor(x => x.PageIndex)
            .GreaterThan(0)
            .WithMessage("Page index must be greater than 0");

            RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Page size must be greater than 0");
        }
    }
}

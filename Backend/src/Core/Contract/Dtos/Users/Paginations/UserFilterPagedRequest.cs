using FluentValidation;

namespace Contract.Dtos.Users.Paginations;

public class UserFilterPagedRequest
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 5;
    public string? RoleName { get; set; }
    public string? FullName { get; set; }
}

public class UserFillterPagedRequestValidator : AbstractValidator<UserFilterPagedRequest>
{
    public UserFillterPagedRequestValidator()
    {
        RuleFor(x => x.PageIndex)
            .GreaterThan(0)
            .WithMessage("Page index must be greater than 0");
        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100");
    }
}

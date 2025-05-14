using FluentValidation;

namespace Contract.Dtos.Users.Requests;

public class EditUserRequest
{
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public int RoleId { get; set; }
}

public class EditUserRequestValidator : AbstractValidator<EditUserRequest>
{
    public EditUserRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Full name is required")
            .MaximumLength(50)
            .WithMessage("Full name must not exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email format");

        RuleFor(x => x.RoleId)
            .NotEmpty()
            .WithMessage("Role ID is required");
    }
}

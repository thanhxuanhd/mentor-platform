using FluentValidation;

namespace Contract.Dtos.Users.Requests;

public class EditUserRequest
{
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Role { get; set; } = default!;
}

public class EditUserRequestValidator : AbstractValidator<EditUserRequest>
{
    public EditUserRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Full name is required")
            .MaximumLength(50)
            .WithMessage("Full name must not exceed 50 characters");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email format");

        RuleFor(x => x.Role)
            .NotEmpty()
            .WithMessage("Role is required");
    }
}

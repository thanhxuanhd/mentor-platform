using FluentValidation;

namespace Contract.Dtos.Authentication.Requests;

public record SignUpRequest(string Password, string ConfirmPassword, string Email, int RoleId);

public class SignUpRequestValidator : AbstractValidator<SignUpRequest>
{
    public SignUpRequestValidator()
    {
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Please enter your new password")
            .Length(8, 32).WithMessage("Password should be in 8-32 characters")
            .Matches(@"^[^ ]*$").WithMessage("Password should not include space characters")
            .Matches(@"[A-Za-z]").WithMessage("Password must include letters")
            .Matches(@"[0-9]").WithMessage("Password must include numbers")
            .Matches(@"[\p{P}\p{S}]").WithMessage("Password must include at least one symbol.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Please confirm your new password")
            .Equal(x => x.Password).WithMessage("Passwords do not match");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Please enter your email")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.RoleId)
            .GreaterThan(0).WithMessage("Invalid Role ID");
    }

}
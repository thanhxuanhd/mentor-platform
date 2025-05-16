using FluentValidation;

namespace Contract.Dtos.Categories.Requests
{
    public class CategoryRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool Status { get; set; }
    }

    public class CategoryRequestValidator : AbstractValidator<CategoryRequest>
    {
        public CategoryRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .MaximumLength(50)
                .WithMessage("Name must not exceed 50 characters.");
            RuleFor(x => x.Description)
                .MaximumLength(256)
                .WithMessage("Description must not exceed 256 characters.");
        }
    }
}

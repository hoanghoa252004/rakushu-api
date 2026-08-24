using FluentValidation;

namespace Rakushu.Application.Usecases.Auth.Register;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
	public RegisterCommandValidator()
	{
		RuleFor(x => x.Username)
			.NotEmpty().WithMessage("Username is required.")
			.MinimumLength(3).WithMessage("Username must be at least 3 characters.")
			.MaximumLength(50).WithMessage("Username cannot exceed 50 characters.")
			.Matches("^[a-zA-Z0-9_.-]+$").WithMessage("Username can only contain alphanumeric characters, dots, underscores, or hyphens.");

		RuleFor(x => x.Email)
			.NotEmpty().WithMessage("Email is required.")
			.EmailAddress().WithMessage("Email format is invalid.")
			.MaximumLength(256).WithMessage("Email cannot exceed 256 characters.");

		RuleFor(x => x.Password)
			.NotEmpty().WithMessage("Password is required.")
			.MinimumLength(6).WithMessage("Password must be at least 6 characters.")
			.MaximumLength(100).WithMessage("Password cannot exceed 100 characters.");
	}
}

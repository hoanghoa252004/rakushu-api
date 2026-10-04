using FluentValidation;

namespace Rakushu.Application.Usecases.Authentication.Register;

internal sealed class RegisterValidator : AbstractValidator<RegisterCommand>
{
	public RegisterValidator()
	{
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

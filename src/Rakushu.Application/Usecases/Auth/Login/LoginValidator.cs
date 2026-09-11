using FluentValidation;

namespace Rakushu.Application.Usecases.Auth.Login;

internal sealed class LoginValidator : AbstractValidator<LoginCommand>
{
	public LoginValidator()
	{
		RuleFor(x => x.Email)
			.NotEmpty().WithMessage("Email is required.")
			.EmailAddress().WithMessage("Invalid email format.")
			.MaximumLength(256).WithMessage("Email must not exceed 256 characters.");

		RuleFor(x => x.Password)
			.NotEmpty().WithMessage("Password is required.")
			.MinimumLength(6).WithMessage("Password must be at least 1 characters long.")
			.MaximumLength(100).WithMessage("Password cannot exceed 100 characters."); ;
	}
}

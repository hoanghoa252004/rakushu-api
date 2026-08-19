using FluentValidation;

namespace Rakushu.Application.Usecases.Auth.Login;

internal sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
	public LoginCommandValidator()
	{
		//RuleFor(x => x.Email)
		//	.NotEmpty().WithMessage("Email is required.")
		//	.EmailAddress().WithMessage("Invalid email format.");

		//RuleFor(x => x.Password)
		//	.MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
		//	.MaximumLength(100).WithMessage("Password must not exceed 100 characters.");
	}
}

using FluentValidation;

namespace Rakushu.Application.Usecases.Admin.Users.CreateUser;

public sealed class AdminCreateUserCommandValidator : AbstractValidator<AdminCreateUserCommand>
{
	public AdminCreateUserCommandValidator()
	{
		RuleFor(x => x.Username)
			.NotEmpty().WithMessage("Username is required.")
			.MinimumLength(3).WithMessage("Username must be at least 3 characters.")
			.MaximumLength(50).WithMessage("Username cannot exceed 50 characters.");

		RuleFor(x => x.Email)
			.NotEmpty().WithMessage("Email is required.")
			.EmailAddress().WithMessage("Email format is invalid.")
			.MaximumLength(256).WithMessage("Email cannot exceed 256 characters.");

		RuleFor(x => x.Password)
			.NotEmpty().WithMessage("Password is required.")
			.MinimumLength(6).WithMessage("Password must be at least 6 characters.");

		RuleFor(x => x.RoleId)
			.NotEmpty().WithMessage("RoleId is required.");
	}
}

using FluentValidation;

namespace Rakushu.Application.Usecases.Users.CreateUser;

public sealed class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
	public CreateUserValidator()
	{
		RuleFor(x => x.Email)
			.NotEmpty().WithMessage("Email is required.")
			.EmailAddress().WithMessage("Email format is invalid.")
			.MaximumLength(256).WithMessage("Email cannot exceed 256 characters.");

		RuleFor(x => x.Password)
			.NotEmpty().WithMessage("Password is required.")
			.MinimumLength(6).WithMessage("Password must be at least 6 characters.")
			.MaximumLength(100).WithMessage("Password cannot exceed 100 characters.");


		RuleFor(x => x.RoleId)
			.NotEmpty().WithMessage("RoleId is required.");

		RuleFor(x => x.FullName)
			.NotEmpty().WithMessage("FullName is required.")
			.MaximumLength(50).WithMessage("FullName cannot exceed 50 characters.");
	}
}

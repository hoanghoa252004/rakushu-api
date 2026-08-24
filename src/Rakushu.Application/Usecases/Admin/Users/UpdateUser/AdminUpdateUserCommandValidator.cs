using FluentValidation;

namespace Rakushu.Application.Usecases.Admin.Users.UpdateUser;

public sealed class AdminUpdateUserCommandValidator : AbstractValidator<AdminUpdateUserCommand>
{
	public AdminUpdateUserCommandValidator()
	{
		RuleFor(x => x.UserId)
			.NotEmpty().WithMessage("UserId is required.");

		RuleFor(x => x.Username)
			.NotEmpty().WithMessage("Username is required.")
			.MinimumLength(3).WithMessage("Username must be at least 3 characters.")
			.MaximumLength(50).WithMessage("Username cannot exceed 50 characters.");

		RuleFor(x => x.Email)
			.NotEmpty().WithMessage("Email is required.")
			.EmailAddress().WithMessage("Email format is invalid.")
			.MaximumLength(256).WithMessage("Email cannot exceed 256 characters.");

		RuleFor(x => x.RoleId)
			.NotEmpty().WithMessage("RoleId is required.");

		RuleFor(x => x.DisplayName)
			.NotEmpty().WithMessage("DisplayName is required.")
			.MaximumLength(100).WithMessage("DisplayName cannot exceed 100 characters.");
	}
}

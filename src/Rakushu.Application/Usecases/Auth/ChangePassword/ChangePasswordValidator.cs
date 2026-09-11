using FluentValidation;

namespace Rakushu.Application.Usecases.Auth.ChangePassword;

internal sealed class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
{
	public ChangePasswordValidator()
	{
		RuleFor(x => x.CurrentPassword)
			.NotEmpty().WithMessage("Current password is required.");

		RuleFor(x => x.NewPassword)
			.NotEmpty().WithMessage("New password is required.")
			.MinimumLength(6).WithMessage("New password must be at least 6 characters.")
			.MaximumLength(100).WithMessage("New password cannot exceed 100 characters.");
	}
}

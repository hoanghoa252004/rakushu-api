using FluentValidation;

namespace Rakushu.Application.Usecases.Authentication.ResetPassword;

internal sealed class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
{
	public ResetPasswordValidator()
	{
		RuleFor(x => x.Email)
			.NotEmpty().WithMessage("Email is required.")
			.EmailAddress().WithMessage("Email is invalid.");

		RuleFor(x => x.Code)
			.NotEmpty().WithMessage("Code is required.")
			.Matches(@"^\d{6}$").WithMessage("Code must be a 6-digit number.");

		RuleFor(x => x.NewPassword)
			.NotEmpty().WithMessage("New password is required.")
			.MinimumLength(6).WithMessage("New password must be at least 6 characters.")
			.MaximumLength(100).WithMessage("New password cannot exceed 100 characters.");
	}
}

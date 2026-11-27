using FluentValidation;

namespace Rakushu.Application.Usecases.Authentication.SendPasswordResetCode;

internal sealed class SendPasswordResetCodeValidator : AbstractValidator<SendPasswordResetCodeCommand>
{
	public SendPasswordResetCodeValidator()
	{
		RuleFor(x => x.Email)
			.NotEmpty().WithMessage("Email is required.")
			.EmailAddress().WithMessage("Email is invalid.");
	}
}

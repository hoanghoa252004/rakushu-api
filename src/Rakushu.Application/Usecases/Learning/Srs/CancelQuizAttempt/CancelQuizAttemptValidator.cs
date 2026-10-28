using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Srs.CancelQuizAttempt;

internal sealed class CancelQuizAttemptValidator : AbstractValidator<CancelQuizAttemptCommand>
{
	public CancelQuizAttemptValidator()
	{
		RuleFor(x => x.AttemptId)
			.NotEmpty()
			.WithMessage("Attempt ID cannot be empty.");
	}
}

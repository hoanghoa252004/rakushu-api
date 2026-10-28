using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Srs.SubmitQuizAttempt;

internal sealed class SubmitQuizAttemptValidator : AbstractValidator<SubmitQuizAttemptCommand>
{
	public SubmitQuizAttemptValidator()
	{
		RuleFor(x => x.AttemptId)
			.NotEmpty()
			.WithMessage("Attempt ID is required.");

		RuleFor(x => x.Answers)
			.NotNull()
			.WithMessage("Answers list cannot be null.");
	}
}

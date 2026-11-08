using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.SubmitQuizAttempt;

public sealed class SubmitQuizAttemptValidator : AbstractValidator<SubmitQuizAttemptCommand>
{
	public SubmitQuizAttemptValidator()
	{
		RuleFor(x => x.AttemptId)
			.NotEmpty()
			.WithMessage("Attempt ID cannot be empty.");
	}
}

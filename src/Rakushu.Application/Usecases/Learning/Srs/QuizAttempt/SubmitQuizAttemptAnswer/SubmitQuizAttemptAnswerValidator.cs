using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.SubmitQuizAttemptAnswer;

public sealed class SubmitQuizAttemptAnswerValidator : AbstractValidator<SubmitQuizAttemptAnswerCommand>
{
	public SubmitQuizAttemptAnswerValidator()
	{
		RuleFor(x => x.AttemptId)
			.NotEmpty()
			.WithMessage("Attempt ID cannot be empty.");

		RuleFor(x => x.ItemId)
			.NotEmpty()
			.WithMessage("Item ID cannot be empty.");

		RuleFor(x => x.SelectedAnswer)
			.NotEmpty()
			.WithMessage("Selected answer cannot be empty. Send 'SKIP' to skip this question.");

		RuleFor(x => x.TimeSpentMs)
			.GreaterThanOrEqualTo(0)
			.WithMessage("Time spent must be non-negative.");
	}
}

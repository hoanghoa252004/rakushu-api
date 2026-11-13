using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.DeleteQuizAttempt;

public sealed class DeleteQuizAttemptValidator : AbstractValidator<DeleteQuizAttemptCommand>
{
	public DeleteQuizAttemptValidator()
	{
		RuleFor(x => x.AttemptId)
			.NotEmpty()
			.WithMessage("Attempt ID cannot be empty.");
	}
}

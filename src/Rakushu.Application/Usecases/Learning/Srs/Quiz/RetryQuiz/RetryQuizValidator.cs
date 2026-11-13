using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Srs.Quiz.RetryQuiz;

public sealed class RetryQuizValidator : AbstractValidator<RetryQuizCommand>
{
	public RetryQuizValidator()
	{
		RuleFor(x => x.QuizId)
			.NotEmpty()
			.WithMessage("Quiz ID cannot be empty.");
	}
}

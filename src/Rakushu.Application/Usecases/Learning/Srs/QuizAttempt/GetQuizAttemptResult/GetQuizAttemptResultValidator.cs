using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.GetQuizAttemptResult;

public sealed class GetQuizAttemptResultValidator : AbstractValidator<GetQuizAttemptResultQuery>
{
	public GetQuizAttemptResultValidator()
	{
		RuleFor(x => x.AttemptId)
			.NotEmpty()
			.WithMessage("Attempt ID cannot be empty.");
	}
}

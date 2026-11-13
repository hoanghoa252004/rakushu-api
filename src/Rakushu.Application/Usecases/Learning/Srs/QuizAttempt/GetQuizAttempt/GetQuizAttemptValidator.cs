using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.GetQuizAttempt;

public sealed class GetQuizAttemptValidator : AbstractValidator<GetQuizAttemptQuery>
{
	public GetQuizAttemptValidator()
	{
		RuleFor(x => x.AttemptId)
			.NotEmpty()
			.WithMessage("Attempt ID cannot be empty.");
	}
}

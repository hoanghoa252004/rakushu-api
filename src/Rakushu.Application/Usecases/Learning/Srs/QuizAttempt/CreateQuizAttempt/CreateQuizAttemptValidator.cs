using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.CreateQuizAttempt;

public sealed class CreateQuizAttemptValidator : AbstractValidator<CreateQuizAttemptCommand>
{
	public CreateQuizAttemptValidator()
	{
		RuleFor(x => x.ItemIds)
			.NotNull()
			.NotEmpty()
			.WithMessage("At least one item must be selected.");
	}
}

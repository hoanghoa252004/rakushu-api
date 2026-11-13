using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.CreateAttemptDetail;

public sealed class CreateAttemptDetailValidator : AbstractValidator<CreateAttemptDetailCommand>
{
	public CreateAttemptDetailValidator()
	{
		RuleFor(x => x.AttemptId)
			.NotEmpty()
			.WithMessage("Attempt ID cannot be empty.");

		RuleFor(x => x.QuizItemId)
			.NotEmpty()
			.WithMessage("Quiz item ID cannot be empty.");

		RuleFor(x => x.SelectedAnswer)
			.NotEmpty()
			.WithMessage("Selected answer cannot be empty.")
			.Must(answer => !QuizAnswerHelper.IsSkip(answer))
			.WithMessage("Selected answer cannot be skipped.");

		RuleFor(x => x.TimeSpentMs)
			.GreaterThanOrEqualTo(0)
			.WithMessage("Time spent must be greater than or equal to 0.");
	}
}

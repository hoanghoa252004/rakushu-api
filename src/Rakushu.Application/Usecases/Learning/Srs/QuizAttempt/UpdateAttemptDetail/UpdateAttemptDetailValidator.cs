using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.UpdateAttemptDetail;

public sealed class UpdateAttemptDetailValidator : AbstractValidator<UpdateAttemptDetailCommand>
{
	public UpdateAttemptDetailValidator()
	{
		RuleFor(x => x.AttemptId)
			.NotEmpty()
			.WithMessage("Attempt ID cannot be empty.");

		RuleFor(x => x.DetailId)
			.NotEmpty()
			.WithMessage("Detail ID cannot be empty.");

		RuleFor(x => x.TimeSpentMs)
			.GreaterThanOrEqualTo(0)
			.WithMessage("Time spent must be greater than or equal to 0.");
	}
}

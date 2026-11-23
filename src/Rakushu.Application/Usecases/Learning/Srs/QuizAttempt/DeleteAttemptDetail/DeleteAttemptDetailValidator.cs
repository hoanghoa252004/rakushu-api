using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.DeleteAttemptDetail;

public sealed class DeleteAttemptDetailValidator : AbstractValidator<DeleteAttemptDetailCommand>
{
	public DeleteAttemptDetailValidator()
	{
		RuleFor(x => x.AttemptId)
			.NotEmpty()
			.WithMessage("Attempt ID cannot be empty.");

		RuleFor(x => x.DetailId)
			.NotEmpty()
			.WithMessage("Detail ID cannot be empty.");
	}
}

using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Srs.CreateQuizAttempt;

internal sealed class CreateQuizAttemptValidator : AbstractValidator<CreateQuizAttemptCommand>
{
	public CreateQuizAttemptValidator()
	{
		RuleFor(x => x.ItemIds)
			.NotEmpty()
			.WithMessage("At least one item must be selected to create a quiz.")
			.Must(x => x != null && x.Count <= 100)
			.WithMessage("You cannot select more than 100 items for a single quiz.");

		RuleForEach(x => x.ItemIds)
			.NotEmpty()
			.WithMessage("Item ID cannot be empty.");
	}
}

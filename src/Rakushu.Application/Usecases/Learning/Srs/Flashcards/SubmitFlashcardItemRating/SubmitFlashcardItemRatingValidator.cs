using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards.SubmitFlashcardItemRating;

internal sealed class SubmitFlashcardItemRatingValidator : AbstractValidator<SubmitFlashcardItemRatingCommand>
{
	public SubmitFlashcardItemRatingValidator()
	{
		RuleFor(x => x.SessionId)
			.NotEmpty()
			.WithMessage("Session ID is required.");

		RuleFor(x => x.ItemId)
			.NotEmpty()
			.WithMessage("Item ID is required.");

		RuleFor(x => x.Rating)
			.NotEmpty()
			.Must(r => r.Equals("Forgot", StringComparison.OrdinalIgnoreCase) ||
			           r.Equals("Hard", StringComparison.OrdinalIgnoreCase) ||
			           r.Equals("Easy", StringComparison.OrdinalIgnoreCase))
			.WithMessage("Rating must be 'Forgot', 'Hard', or 'Easy'.");
	}
}

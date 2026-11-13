using FluentValidation;
using Rakushu.Domain.Entities.Srs.FlashcardSession;

namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards.CreateFlashcardSessionItem;

public sealed class CreateFlashcardSessionItemValidator : AbstractValidator<CreateFlashcardSessionItemCommand>
{
	private static readonly string[] AllowedRatings = ["Forgot", "Hard", "Easy"];

	public CreateFlashcardSessionItemValidator()
	{
		RuleFor(x => x.SessionId)
			.NotEmpty()
			.WithMessage("Session ID is required.");

		RuleFor(x => x.SavedItemId)
			.NotEmpty()
			.WithMessage("Saved item ID is required.");

		RuleFor(x => x.Rating)
			.NotEmpty()
			.Must(r => AllowedRatings.Contains(r, StringComparer.OrdinalIgnoreCase))
			.WithErrorCode(FlashcardSessionErrors.InvalidRating.Code)
			.WithMessage("Rating must be 'Forgot', 'Hard', or 'Easy'.");
	}
}

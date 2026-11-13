using FluentValidation;
using Rakushu.Domain.Entities.Srs.FlashcardSession;

namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards.UpdateFlashcardSessionItem;

public sealed class UpdateFlashcardSessionItemValidator : AbstractValidator<UpdateFlashcardSessionItemCommand>
{
	private static readonly string[] AllowedRatings = ["Forgot", "Hard", "Easy"];

	public UpdateFlashcardSessionItemValidator()
	{
		RuleFor(x => x.SessionId)
			.NotEmpty()
			.WithMessage("Session ID is required.");

		RuleFor(x => x.ItemId)
			.NotEmpty()
			.WithMessage("Item ID is required.");

		RuleFor(x => x.Rating)
			.NotEmpty()
			.Must(r => AllowedRatings.Contains(r, StringComparer.OrdinalIgnoreCase))
			.WithErrorCode(FlashcardSessionErrors.InvalidRating.Code)
			.WithMessage("Rating must be 'Forgot', 'Hard', or 'Easy'.");
	}
}

using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards.SubmitFlashcardSession;

internal sealed class SubmitFlashcardSessionValidator : AbstractValidator<SubmitFlashcardSessionCommand>
{
	public SubmitFlashcardSessionValidator()
	{
		RuleFor(x => x.SessionId)
			.NotEmpty()
			.WithMessage("Session ID cannot be empty.");

		RuleFor(x => x.Ratings)
			.NotNull()
			.NotEmpty()
			.WithMessage("Ratings list cannot be empty.");

		RuleForEach(x => x.Ratings).ChildRules(rating =>
		{
			rating.RuleFor(r => r.SavedItemId)
				.NotEmpty()
				.WithMessage("SavedItemId cannot be empty.");

			rating.RuleFor(r => r.Rating)
				.NotEmpty()
				.Must(r => r.Equals("Forgot", StringComparison.OrdinalIgnoreCase) ||
						   r.Equals("Hard", StringComparison.OrdinalIgnoreCase) ||
						   r.Equals("Easy", StringComparison.OrdinalIgnoreCase))
				.WithMessage("Rating must be 'Forgot', 'Hard', or 'Easy'.");
		});
	}
}

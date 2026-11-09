using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards.SubmitFlashcardSession;

public sealed class SubmitFlashcardSessionValidator : AbstractValidator<SubmitFlashcardSessionCommand>
{
	public SubmitFlashcardSessionValidator()
	{
		RuleFor(x => x.SessionId)
			.NotEmpty()
			.WithMessage("Session ID is required.");
	}
}

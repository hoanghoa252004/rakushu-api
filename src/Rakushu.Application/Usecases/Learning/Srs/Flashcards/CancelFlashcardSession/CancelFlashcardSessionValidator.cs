using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards.CancelFlashcardSession;

internal sealed class CancelFlashcardSessionValidator : AbstractValidator<CancelFlashcardSessionCommand>
{
	public CancelFlashcardSessionValidator()
	{
		RuleFor(x => x.SessionId)
			.NotEmpty()
			.WithMessage("Session ID cannot be empty.");
	}
}

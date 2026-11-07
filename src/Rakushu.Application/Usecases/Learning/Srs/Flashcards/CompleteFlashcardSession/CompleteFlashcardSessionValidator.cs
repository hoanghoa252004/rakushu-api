using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards.CompleteFlashcardSession;

internal sealed class CompleteFlashcardSessionValidator : AbstractValidator<CompleteFlashcardSessionCommand>
{
	public CompleteFlashcardSessionValidator()
	{
		RuleFor(x => x.SessionId)
			.NotEmpty()
			.WithMessage("Session ID is required.");
	}
}

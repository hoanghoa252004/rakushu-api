using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards.DeleteFlashcardSession;

internal sealed class DeleteFlashcardSessionValidator : AbstractValidator<DeleteFlashcardSessionCommand>
{
	public DeleteFlashcardSessionValidator()
	{
		RuleFor(x => x.SessionId)
			.NotEmpty()
			.WithMessage("Session ID cannot be empty.");
	}
}

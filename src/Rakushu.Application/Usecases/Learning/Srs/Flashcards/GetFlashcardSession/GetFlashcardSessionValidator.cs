using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards.GetFlashcardSession;

internal sealed class GetFlashcardSessionValidator : AbstractValidator<GetFlashcardSessionQuery>
{
	public GetFlashcardSessionValidator()
	{
		RuleFor(x => x.SessionId)
			.NotEmpty()
			.WithMessage("Session ID is required.");
	}
}

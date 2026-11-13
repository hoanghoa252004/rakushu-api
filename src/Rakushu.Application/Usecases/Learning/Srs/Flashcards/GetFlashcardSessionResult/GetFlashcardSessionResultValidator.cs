using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards.GetFlashcardSessionResult;

internal sealed class GetFlashcardSessionResultValidator : AbstractValidator<GetFlashcardSessionResultQuery>
{
	public GetFlashcardSessionResultValidator()
	{
		RuleFor(x => x.SessionId)
			.NotEmpty()
			.WithMessage("Session ID is required.");
	}
}

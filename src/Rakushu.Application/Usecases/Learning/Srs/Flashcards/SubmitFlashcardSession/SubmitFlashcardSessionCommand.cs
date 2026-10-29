using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards.SubmitFlashcardSession;

public sealed record SubmitFlashcardSessionCommand(
	Guid SessionId,
	IReadOnlyList<FlashcardItemRatingInput> Ratings) : IRequest<Result<FlashcardSessionResultDto>>;

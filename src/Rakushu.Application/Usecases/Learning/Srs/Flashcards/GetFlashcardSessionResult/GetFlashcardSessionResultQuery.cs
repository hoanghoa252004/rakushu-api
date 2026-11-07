using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards.GetFlashcardSessionResult;

public sealed record GetFlashcardSessionResultQuery(
	Guid SessionId) : IRequest<Result<FlashcardSessionResultDto>>;

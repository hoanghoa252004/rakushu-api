using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards.GetFlashcardSession;

public sealed record GetFlashcardSessionQuery(Guid SessionId) : IRequest<Result<FlashcardSessionDetailDto>>;

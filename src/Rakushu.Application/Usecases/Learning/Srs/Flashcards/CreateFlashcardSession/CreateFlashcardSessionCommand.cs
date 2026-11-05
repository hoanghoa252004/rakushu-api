using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards.CreateFlashcardSession;

public sealed record CreateFlashcardSessionCommand(
	IReadOnlyList<Guid> ItemIds) : IRequest<Result<FlashcardSessionDetailDto>>;

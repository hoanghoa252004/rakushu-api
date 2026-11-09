using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards.UpdateFlashcardSessionItem;

public sealed record UpdateFlashcardSessionItemCommand(
	Guid SessionId,
	Guid ItemId,
	string Rating) : IRequest<Result>;

using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards.CreateFlashcardSessionItem;

public sealed record CreateFlashcardSessionItemCommand(
	Guid SessionId,
	Guid SavedItemId,
	string Rating) : IRequest<Result<Guid>>;

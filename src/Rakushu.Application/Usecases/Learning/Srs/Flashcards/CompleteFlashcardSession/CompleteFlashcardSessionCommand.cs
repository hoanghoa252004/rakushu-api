using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards.CompleteFlashcardSession;

public sealed record CompleteFlashcardSessionCommand(
	Guid SessionId) : IRequest<Result>;

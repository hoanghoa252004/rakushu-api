using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards.CancelFlashcardSession;

public sealed record CancelFlashcardSessionCommand(Guid SessionId) : IRequest<Result>;

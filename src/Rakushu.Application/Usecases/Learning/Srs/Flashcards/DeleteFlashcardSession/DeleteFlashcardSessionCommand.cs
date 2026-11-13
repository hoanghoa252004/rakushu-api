using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards.DeleteFlashcardSession;

public sealed record DeleteFlashcardSessionCommand(Guid SessionId) : IRequest<Result>;

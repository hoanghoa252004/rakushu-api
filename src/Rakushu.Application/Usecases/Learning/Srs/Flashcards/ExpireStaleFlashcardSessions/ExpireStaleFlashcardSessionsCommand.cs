using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards.ExpireStaleFlashcardSessions;

public sealed record ExpireStaleFlashcardSessionsCommand(int BatchSize = 50) : IRequest<Result<int>>;

using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.ExpireStaleQuizAttempts;

public sealed record ExpireStaleQuizAttemptsCommand(int BatchSize = 50) : IRequest<Result<int>>;

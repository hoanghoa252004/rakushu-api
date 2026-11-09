using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.EvaluatePendingQuizAttempts;

public sealed record EvaluatePendingQuizAttemptsCommand(int BatchSize = 10) : IRequest<Result<int>>;

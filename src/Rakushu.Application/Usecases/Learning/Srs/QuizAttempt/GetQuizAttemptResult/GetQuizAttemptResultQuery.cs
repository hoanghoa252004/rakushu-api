using MediatR;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.GetQuizAttemptResult;

public sealed record GetQuizAttemptResultQuery(Guid AttemptId) : IRequest<Result<QuizAttemptResultDto>>;

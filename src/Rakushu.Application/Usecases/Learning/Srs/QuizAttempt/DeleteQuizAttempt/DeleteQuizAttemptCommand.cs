using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.DeleteQuizAttempt;

public sealed record DeleteQuizAttemptCommand(Guid AttemptId) : IRequest<Result>;

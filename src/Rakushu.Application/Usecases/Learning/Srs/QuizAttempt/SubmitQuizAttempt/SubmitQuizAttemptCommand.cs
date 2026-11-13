using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.SubmitQuizAttempt;

public sealed record SubmitQuizAttemptCommand(Guid AttemptId) : IRequest<Result>;

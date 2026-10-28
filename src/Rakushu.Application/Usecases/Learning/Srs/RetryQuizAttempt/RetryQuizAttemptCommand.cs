using MediatR;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.RetryQuizAttempt;

public sealed record RetryQuizAttemptCommand(
	Guid AttemptId,
	string RetryMode = "All") : IRequest<Result<QuizAttemptStartedResponse>>;

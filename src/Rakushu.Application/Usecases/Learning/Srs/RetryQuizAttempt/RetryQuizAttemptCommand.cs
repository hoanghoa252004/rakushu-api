using MediatR;
using Rakushu.Application.Usecases.Learning.Srs.Common;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.RetryQuizAttempt;

public sealed record RetryQuizAttemptCommand(
	Guid AttemptId,
	string RetryMode) : IRequest<Result<QuizAttemptStartedResponse>>;

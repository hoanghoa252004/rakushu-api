using MediatR;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.CreateQuizAttempt;

public sealed record CreateQuizAttemptCommand(
	IReadOnlyList<Guid> ItemIds) : IRequest<Result<QuizAttemptStartedResponse>>;

using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.DeleteAttemptDetail;

public sealed record DeleteAttemptDetailCommand(
	Guid AttemptId,
	Guid DetailId) : IRequest<Result>;

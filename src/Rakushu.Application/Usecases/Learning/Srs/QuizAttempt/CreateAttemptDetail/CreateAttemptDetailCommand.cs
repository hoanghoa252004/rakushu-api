using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.CreateAttemptDetail;

public sealed record CreateAttemptDetailCommand(
	Guid AttemptId,
	Guid QuizItemId,
	string? SelectedAnswer,
	int TimeSpentMs) : IRequest<Result<Guid>>;

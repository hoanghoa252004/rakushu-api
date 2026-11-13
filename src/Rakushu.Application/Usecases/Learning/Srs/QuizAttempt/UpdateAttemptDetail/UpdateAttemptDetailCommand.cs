using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.UpdateAttemptDetail;

public sealed record UpdateAttemptDetailCommand(
	Guid AttemptId,
	Guid DetailId,
	string SelectedAnswer,
	int TimeSpentMs) : IRequest<Result>;

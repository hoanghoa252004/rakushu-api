using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.SubmitQuizAttemptAnswer;

public sealed record SubmitQuizAttemptAnswerCommand(
	Guid AttemptId,
	Guid ItemId,
	string SelectedAnswer,
	int TimeSpentMs) : IRequest<Result>;

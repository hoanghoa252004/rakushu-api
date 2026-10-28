using MediatR;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.SubmitQuizAttempt;

public sealed record QuizAnswerSubmission(
	Guid QuestionId,
	string? SelectedOptionId,
	int? ElapsedMs);

public sealed record SubmitQuizAttemptCommand(
	Guid AttemptId,
	IReadOnlyList<QuizAnswerSubmission> Answers) : IRequest<Result<QuizAttemptResultResponse>>;

using MediatR;
using Rakushu.Application.Usecases.Learning.Srs.Common;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.SubmitQuizAttempt;

public sealed record QuizAnswerSubmission(Guid ItemId, string SelectedOptionId);

public sealed record SubmitQuizAttemptCommand(
    Guid AttemptId,
    IReadOnlyList<QuizAnswerSubmission> Answers)
    : IRequest<Result<QuizAttemptResultResponse>>;

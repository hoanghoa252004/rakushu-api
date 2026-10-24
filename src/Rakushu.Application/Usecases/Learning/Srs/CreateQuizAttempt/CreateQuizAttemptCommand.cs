using MediatR;
using Rakushu.Application.Usecases.Learning.Srs.Common;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.CreateQuizAttempt;

public sealed record CreateQuizAttemptCommand(
	IReadOnlyList<Guid>? ItemIds,
	string QuestionFormat = "MC")
	: IRequest<Result<QuizAttemptStartedResponse>>;

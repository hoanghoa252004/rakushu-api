using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.CreateQuizAttempt;

public sealed record CreateQuizAttemptCommand(
	IReadOnlyList<Guid> ItemIds,
	string? Mode = null,
	bool DiscardActive = false) : IRequest<Result<Guid>>;

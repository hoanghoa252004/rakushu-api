using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Curation.Learner.SubmitIssue;

public sealed record SubmitIssueCommand(
	string TargetType,
	Guid TargetId,
	string Category,
	string? LearnerFeedback,
	int? StartCharIndex = null,
	int? EndCharIndex = null) : IRequest<Result<SubmitIssueResponse>>;

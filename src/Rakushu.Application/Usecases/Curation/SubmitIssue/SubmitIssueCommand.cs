using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Curation.SubmitIssue;

public sealed record SubmitIssueCommand(
	string TargetType,
	Guid TargetId,
	string Category,
	string? LearnerFeedback) : IRequest<Result<SubmitIssueResponse>>;

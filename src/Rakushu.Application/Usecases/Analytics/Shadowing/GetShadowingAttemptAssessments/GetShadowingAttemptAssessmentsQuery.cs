using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Analytics.Shadowing.GetShadowingAttemptAssessments;

public sealed record GetShadowingAttemptAssessmentsQuery(
	Guid SessionId,
	Guid AttemptId) : IRequest<Result<ShadowingAttemptAssessmentsResponseDto>>;

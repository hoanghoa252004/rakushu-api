using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Analytics.Shadowing.GetShadowingAnalytics;

public sealed record GetShadowingAnalyticsQuery(
	string Period = "30d",
	string? TimeZone = null) : IRequest<Result<ShadowingAnalyticsDetailResponseDto>>;

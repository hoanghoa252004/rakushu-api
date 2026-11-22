using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Analytics.Curator.GetCuratorDashboard;

public sealed record GetCuratorDashboardQuery(
	int Days = 30,
	string? TimeZone = null) : IRequest<Result<CuratorDashboardDto>>;

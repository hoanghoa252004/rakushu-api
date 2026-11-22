using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Analytics.Learner.GetLearnerDashboard;

public sealed record GetLearnerDashboardQuery(
	int Days = 30,
	int ForecastDays = 14,
	string? TimeZone = null,
	int? HeatmapYear = null) : IRequest<Result<LearnerDashboardDto>>;

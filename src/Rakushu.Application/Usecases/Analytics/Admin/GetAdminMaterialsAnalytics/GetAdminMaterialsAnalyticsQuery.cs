using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Analytics.Admin.GetAdminMaterialsAnalytics;

public sealed record GetAdminMaterialsAnalyticsQuery(int DaysWindow = 7)
	: IRequest<Result<AdminMaterialsAnalyticsDto>>;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Abstractions.Infrastructure.JobLog;
using Rakushu.Application.Usecases.Processing.ProcessingJobLogs;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.Processing.ProcessingJobLog;

namespace Rakushu.Api.Endpoints.Processing;

internal sealed class ProcessingJobLogEndpoints : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("/api/processing-job-logs")
			.WithTags("ProcessingJobLogs")
			.WithGroupName("material")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator, RoleCodes.SystemAdministrator));

		group.MapGet("/", async (
				[FromQuery] Guid? targetId,
				[FromQuery] ProcessingJobTargetType? targetType,
				[FromQuery] ProcessingJobType? jobType,
				[FromQuery] bool? isSuccess,
				[FromQuery] bool? isFailed,
				ISender sender,
				CancellationToken ct) =>
			{
				var result = await sender.Send(
					new GetProcessingJobLogsQuery(targetId, targetType, jobType, isSuccess, isFailed), ct);
				return result.MatchOk();
			})
			.WithName("GetProcessingJobLogs")
			.WithSummary("Curator")
			.WithDescription("Gets processing job logs filtered by target (id/type), job type and success/failed state, sorted by ascending sequence.")
			.Produces<IReadOnlyList<ProcessingJobLogDetail>>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

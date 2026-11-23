using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.JobLog;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Processing.ProcessingJobLog;

namespace Rakushu.Application.Usecases.Processing.ProcessingJobLogs;

public sealed record GetProcessingJobLogsQuery(
	Guid? TargetId,
	ProcessingJobTargetType? TargetType,
	ProcessingJobType? JobType,
	bool? IsSuccess,
	bool? IsFailed,
	Guid? ParentJobLogId) : IRequest<Result<IReadOnlyList<ProcessingJobLogDetail>>>;

internal sealed class GetProcessingJobLogsHandler(IProcessingJobLogService jobLogService)
	: IRequestHandler<GetProcessingJobLogsQuery, Result<IReadOnlyList<ProcessingJobLogDetail>>>
{
	public async Task<Result<IReadOnlyList<ProcessingJobLogDetail>>> Handle(
		GetProcessingJobLogsQuery request,
		CancellationToken cancellationToken)
	{
		var logs = await jobLogService.SearchAsync(
			new ProcessingJobLogFilter(
				request.TargetId,
				request.TargetType,
				request.JobType,
				request.IsSuccess,
				request.IsFailed,
				request.ParentJobLogId),
			cancellationToken);

		return Result.Success(logs);
	}
}

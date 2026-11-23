using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities.Processing.ProcessingJobLog;

public sealed class ProcessingJobLogId : StronglyTypedId<Guid>
{
	private ProcessingJobLogId(Guid value) : base(value)
	{
	}

	public static ProcessingJobLogId Create() => new(Guid.NewGuid());

	public static ProcessingJobLogId From(Guid value) => new(value);
}

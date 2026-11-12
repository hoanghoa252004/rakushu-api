using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities.Curation.Report;

public sealed class ReportId : StronglyTypedId<Guid>
{
	private ReportId(Guid value) : base(value)
	{
	}

	public static ReportId Create() => new(Guid.NewGuid());
	public static ReportId From(Guid value) => new(value);
}

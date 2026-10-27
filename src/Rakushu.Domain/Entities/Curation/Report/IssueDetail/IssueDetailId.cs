using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities.Curation.Report.IssueDetail;

public sealed class IssueDetailId : StronglyTypedId<Guid>
{
	private IssueDetailId(Guid value) : base(value)
	{
	}

	public static IssueDetailId Create() => new(Guid.NewGuid());
	public static IssueDetailId From(Guid value) => new(value);
}

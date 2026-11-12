using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities.Curation.IssueCategory;

public sealed class IssueCategoryId : StronglyTypedId<Guid>
{
	private IssueCategoryId(Guid value) : base(value)
	{
	}

	public static IssueCategoryId Create() => new(Guid.NewGuid());
	public static IssueCategoryId From(Guid value) => new(value);
}

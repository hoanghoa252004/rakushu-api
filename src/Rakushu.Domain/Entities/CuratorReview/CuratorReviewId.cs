using Rakushu.Domain.Common;
using System;

namespace Rakushu.Domain.Entities.CuratorReview;

public sealed class CuratorReviewId : StronglyTypedId<Guid>
{
	private CuratorReviewId(Guid value) : base(value)
	{
	}

	public static CuratorReviewId Create() => new(Guid.NewGuid());

	public static CuratorReviewId From(Guid value) => new(value);
}

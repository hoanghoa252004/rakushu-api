using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities.Learning.LearnerVideoProgress;

public sealed class LearnerVideoProgressId : StronglyTypedId<Guid>
{
	private LearnerVideoProgressId(Guid value) : base(value)
	{
	}

	public static LearnerVideoProgressId Create() => new(Guid.NewGuid());
	public static LearnerVideoProgressId From(Guid value) => new(value);
}

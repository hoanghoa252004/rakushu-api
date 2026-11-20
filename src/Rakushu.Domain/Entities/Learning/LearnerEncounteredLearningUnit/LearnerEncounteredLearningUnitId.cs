using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities.Learning.LearnerEncounteredLearningUnit;

public sealed class LearnerEncounteredLearningUnitId : StronglyTypedId<Guid>
{
	private LearnerEncounteredLearningUnitId(Guid value) : base(value)
	{
	}

	public static LearnerEncounteredLearningUnitId Create() => new(Guid.NewGuid());
	public static LearnerEncounteredLearningUnitId From(Guid value) => new(value);
}

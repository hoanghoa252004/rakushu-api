using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities.Learning.LearnerEncounterEvent;

public sealed class LearnerEncounterEventId : StronglyTypedId<Guid>
{
	private LearnerEncounterEventId(Guid value) : base(value)
	{
	}

	public static LearnerEncounterEventId Create() => new(Guid.NewGuid());
	public static LearnerEncounterEventId From(Guid value) => new(value);
}

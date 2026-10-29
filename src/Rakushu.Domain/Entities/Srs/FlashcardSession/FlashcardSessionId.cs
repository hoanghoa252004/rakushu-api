using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities.Srs.FlashcardSession;

public sealed class FlashcardSessionId : StronglyTypedId<Guid>
{
	private FlashcardSessionId(Guid value) : base(value)
	{
	}

	public static FlashcardSessionId Create() => new(Guid.NewGuid());
	public static FlashcardSessionId From(Guid value) => new(value);
}

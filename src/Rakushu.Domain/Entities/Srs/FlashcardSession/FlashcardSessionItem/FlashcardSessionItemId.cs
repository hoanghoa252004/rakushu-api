using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities.Srs.FlashcardSession.FlashcardSessionItem;

public sealed class FlashcardSessionItemId : StronglyTypedId<Guid>
{
	private FlashcardSessionItemId(Guid value) : base(value)
	{
	}

	public static FlashcardSessionItemId Create() => new(Guid.NewGuid());
	public static FlashcardSessionItemId From(Guid value) => new(value);
}

using Rakushu.Domain.Common;
using System;

namespace Rakushu.Domain.Entities.DictionaryEntry;

public sealed class DictionaryEntryId : StronglyTypedId<Guid>
{
	private DictionaryEntryId(Guid value) : base(value)
	{
	}

	public static DictionaryEntryId Create() => new(Guid.NewGuid());

	public static DictionaryEntryId From(Guid value) => new(value);
}

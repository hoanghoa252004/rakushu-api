using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.LinguisticMetadata.UniversalPartOfSpeech;

public sealed class UniversalPartOfSpeechId : StronglyTypedId<Guid>
{
	private UniversalPartOfSpeechId(Guid value) : base(value)
	{
	}

	public static UniversalPartOfSpeechId Create() => new(Guid.NewGuid());

	public static UniversalPartOfSpeechId From(Guid value) => new(value);
}
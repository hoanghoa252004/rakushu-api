using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Linguistic.JapanesePartOfSpeech;

public sealed class JapanesePartOfSpeechId : StronglyTypedId<Guid>
{
	private JapanesePartOfSpeechId(Guid value) : base(value)
	{
	}

	public static JapanesePartOfSpeechId Create() => new(Guid.NewGuid());

	public static JapanesePartOfSpeechId From(Guid value) => new(value);
}